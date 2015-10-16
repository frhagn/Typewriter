using System;
using System.Diagnostics;
using System.Linq;
using EnvDTE;
using Typewriter.CodeModel.Implementation;
using Typewriter.Metadata.Providers;
using Typewriter.VisualStudio;
using VSLangProj;

namespace Typewriter.Generation.Controllers
{
    public class GenerationController
    {
        private readonly DTE dte;
        private readonly IMetadataProvider metadataProvider;
        private readonly TemplateController templateController;
        private readonly IEventQueue eventQueue;
        
        public GenerationController(DTE dte, IMetadataProvider metadataProvider, TemplateController templateController, IEventQueue eventQueue)
        {
            this.dte = dte;
            this.metadataProvider = metadataProvider;
            this.templateController = templateController;
            this.eventQueue = eventQueue;
        }


        public void OnTemplateChanged(string templatePath)
        {

            Log.Debug("{0} queued {1}", GenerationType.Template, templatePath);

            var projectItem = dte.Solution.FindProjectItem(templatePath);

            var vsProject = projectItem.ContainingProject.Object as VSProject;
            var referencedItems = vsProject.GetReferencedProjectItems(Constants.CsExtension).Select(m => m.Path()).ToArray();

            Log.Debug(" Will Check/Render {0} .cs files in referenced projects", referencedItems.Length);

            eventQueue.Enqueue(() =>
            {
                var stopwatch = Stopwatch.StartNew();

                var template = templateController.TemplatesLoaded
                    ? templateController.Templates.FirstOrDefault(m => m.TemplatePath.Equals(projectItem.Path(), StringComparison.InvariantCultureIgnoreCase))
                    : null;

                if (template == null)
                {
                    template = new Template(projectItem);
                    templateController.ResetTemplates();
                }
                else
                {
                    template.Reload();
                }

                foreach (var path in referencedItems)
                {

                    var metadata = metadataProvider.GetFile(path);
                    var file = new FileImpl(metadata);

                    template.RenderFile(file);

                    if (template.HasCompileException)
                    {
                        break;
                    }
                }

                template.SaveProjectFile();


                stopwatch.Stop();
                Log.Debug("{0} processed {1} in {2}ms", GenerationType.Template, templatePath, stopwatch.ElapsedMilliseconds);

            });
        }

        public void OnCsFileChanged(string[] paths)
        {
            Enqueue(GenerationType.Render, paths, path => new FileImpl(metadataProvider.GetFile(path)), (path, template) => template.RenderFile(path));
        }


        public void OnCsFileDeleted(string[] paths)
        {
            Enqueue(GenerationType.Delete, paths, (path, template) => template.DeleteFile(path));

        }
        private void Enqueue(GenerationType type, string[] paths, Action<string, Template> action)
        {
            Enqueue(type, paths, (s, i) => s, action);
        }

        public void OnCsFileRenamed(string[] newPaths, string[] oldPaths)
        {
            Enqueue(GenerationType.Rename, newPaths, (path, fileIndex) => new { OldPath = oldPaths[fileIndex], NewPath = path },
                (item, template) => template.RenameFile(item.OldPath, item.NewPath));
        }

        private void Enqueue<T>(GenerationType type, string[] paths, Func<string, T> transform, Action<T, Template> action)
        {
            Enqueue(type, paths, (s, i) => transform(s), action);
        }

        private void Enqueue<T>(GenerationType type, string[] paths, Func<string, int, T> transform, Action<T, Template> action)
        {
            var templates = templateController.Templates.Where(m => !m.HasCompileException).ToArray();
            if (!templates.Any())
            {
                return;
            }
            Log.Debug("{0} queued {1}", type, string.Join(", ", paths));

            eventQueue.Enqueue(() =>
            {
                var stopwatch = Stopwatch.StartNew();

                paths.Select(transform).ForEach(path => templates.ForEach(template => action(path, template)));

                templates.GroupBy(m => m.ProjectFullName).ForEach(template => template.First().SaveProjectFile());

                stopwatch.Stop();
                Log.Debug("{0} processed {1} in {2}ms", type, string.Join(", ", paths), stopwatch.ElapsedMilliseconds);

            });
        }


    }
}