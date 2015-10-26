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
        private readonly DTE _dte;
        private readonly IMetadataProvider _metadataProvider;
        private readonly TemplateController _templateController;
        private readonly IEventQueue _eventQueue;
        
        public GenerationController(DTE dte, IMetadataProvider metadataProvider, TemplateController templateController, IEventQueue eventQueue)
        {
            _dte = dte;
            _metadataProvider = metadataProvider;
            _templateController = templateController;
            _eventQueue = eventQueue;
        }


        public void OnTemplateChanged(string templatePath)
        {
            
            Log.Debug("{0} queued {1}", GenerationType.Template, templatePath);

            ErrorList.Clear();

            var projectItem = _dte.Solution.FindProjectItem(templatePath);

            var template = _templateController.GetTemplate(projectItem);
            var filesToRender = template.GetFilesToRender();
            Log.Debug(" Will Check/Render {0} .cs files in referenced projects", filesToRender.Count);
            
            _eventQueue.Enqueue(() =>
            {
                var stopwatch = Stopwatch.StartNew();
                

                foreach (var path in filesToRender)
                {
                    var metadata = _metadataProvider.GetFile(path);
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
            Enqueue(GenerationType.Render, paths, path => new FileImpl(_metadataProvider.GetFile(path)), (file, template) =>
            {
                if (template.ShouldRenderFile(file.FullName))
                {
                    template.RenderFile(file);
                }
            });
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
            Enqueue(GenerationType.Rename, newPaths, (path, fileIndex) => new { OldPath = oldPaths[fileIndex], NewPath = path, NewFile = new FileImpl(_metadataProvider.GetFile(path)) },
                (item, template) => template.RenameFile(item.NewFile, item.OldPath, item.NewPath));
        }

        private void Enqueue<T>(GenerationType type, string[] paths, Func<string, T> transform, Action<T, Template> action)
        {
            Enqueue(type, paths, (s, i) => transform(s), action);
        }

        private void Enqueue<T>(GenerationType type, string[] paths, Func<string, int, T> transform, Action<T, Template> action)
        {
            var templates = _templateController.Templates.Where(m => !m.HasCompileException).ToArray();
            if (!templates.Any())
            {
                return;
            }
            Log.Debug("{0} queued {1}", type, string.Join(", ", paths));

            _eventQueue.Enqueue(() =>
            {

                var stopwatch = Stopwatch.StartNew();

                paths.ForEach((path,i) =>
                {
                    var item = transform(path, i);

                    templates.ForEach(template => action(item, template));
                });

                templates.GroupBy(m => m.ProjectFullName).ForEach(template => template.First().SaveProjectFile());

                stopwatch.Stop();
                Log.Debug("{0} processed {1} in {2}ms", type, string.Join(", ", paths), stopwatch.ElapsedMilliseconds);

            });
        }


    }
}