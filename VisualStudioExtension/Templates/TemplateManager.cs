using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using EnvDTE;
using Typewriter.CodeModel.CodeDom;
using Typewriter.VisualStudio;
using VSLangProj;

namespace Typewriter.Templates
{
    public interface ITemplateManager
    {
        IEnumerable<ITemplate> Templates { get; }
    }

    public class TemplateManager : ITemplateManager
    {
        private const string templateExtension = ".tst";

        private readonly ILog log;
        private readonly DTE dte;
        private readonly IEventQueue eventQueue;

        private bool solutionOpen;
        private List<Template> templates;

        public TemplateManager(ILog log, DTE dte, ISolutionMonitor solutionMonitor, IEventQueue eventQueue)
        {
            this.log = log;
            this.dte = dte;
            this.eventQueue = eventQueue;

            solutionMonitor.SolutionOpened += (sender, args) => SolutionOpened();
            solutionMonitor.SolutionClosed += (sender, args) => SolutionClosed();
            solutionMonitor.ProjectAdded += (o, e) => ProjectChanged();
            solutionMonitor.ProjectRemoved += (o, e) => ProjectChanged();

            solutionMonitor.FileAdded += (o, e) => FileChanged(e.Path);
            solutionMonitor.FileChanged += (o, e) => FileChanged(e.Path);
            solutionMonitor.FileDeleted += (o, e) => FileChanged(e.Path);
            solutionMonitor.FileRenamed += (o, e) => FileRenamed(e.OldPath, e.NewPath);
        }

        private void SolutionOpened()
        {
            log.Debug("Solution Opened");
            solutionOpen = true;
        }

        private void SolutionClosed()
        {
            log.Debug("Solution Closed");
            solutionOpen = false;
        }

        private void ProjectChanged()
        {
            if (solutionOpen == false) return;
            this.templates = null;
        }

        private void FileChanged(string path)
        {
            if (path.EndsWith(templateExtension, StringComparison.InvariantCultureIgnoreCase))
            {
                this.templates = null;

                var projectItem = dte.Solution.FindProjectItem(path);
                var template = new Template(projectItem);

                foreach (var item in GetReferencedProjectItems(projectItem, ".cs"))
                {
                    eventQueue.QueueRender(item.FileNames[1], s =>
                    {
                        try
                        {
                            var stopwatch = Stopwatch.StartNew();
                            log.Debug("Render {0}", s);

                            var file = new FileInfo(log, dte.Solution.FindProjectItem(s));
                            template.Render(file);

                            stopwatch.Stop();
                            log.Debug("Render completed in {0} ms", stopwatch.ElapsedMilliseconds);
                        }
                        catch (Exception exception)
                        {
                            log.Error("Render Exception: {0}", exception.Message);
                        }
                    });
                }
            }
        }

        private void FileRenamed(string oldPath, string newPath)
        {
            FileChanged(oldPath);
            FileChanged(newPath);
        }

        public IEnumerable<ITemplate> Templates
        {
            get
            {
                var stopwatch = Stopwatch.StartNew();

                LoadTemplates();

                stopwatch.Stop();
                log.Debug("Templates loaded in {0} ms", stopwatch.ElapsedMilliseconds);
                stopwatch.Restart();

                ValidateLoadedTemplates();

                stopwatch.Stop();
                log.Debug("Templates validated in {0} ms", stopwatch.ElapsedMilliseconds);
                
                return this.templates;
            }
        }

        private void LoadTemplates()
        {
            if (this.templates == null)
            {
                var items = GetProjectItems(templateExtension);
                this.templates = items.Select(i => new Template(i)).ToList();
            }
        }

        private void ValidateLoadedTemplates()
        {
            foreach (var template in this.templates)
            {
                try
                {
                    template.VerifyProjectItem();
                }
                catch
                {
                    log.Debug("Invalid template");
                    this.templates = null;
                    LoadTemplates();
                    break;
                }
            }
        }

        private IEnumerable<ProjectItem> GetReferencedProjectItems(ProjectItem i, string extension)
        {
            var vsproject = i.ContainingProject.Object as VSProject;
            if (vsproject != null)
            {
                foreach (Reference reference in vsproject.References)
                {
                    var sp = reference.SourceProject;
                    if (sp != null)
                    {
                        foreach (var item in sp.AllProjectItems().Where(item => item.Name.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            yield return item;
                        }
                    }
                }

                foreach (var item in i.ContainingProject.AllProjectItems().Where(item => item.Name.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase)))
                {
                    yield return item;
                }
            }
        }

        private IEnumerable<ProjectItem> GetProjectItems(string extension)
        {
            foreach (var project in dte.Solution.AllProjetcs())
            {
                foreach (var item in project.AllProjectItems())
                {
                    if (item.Name.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase))
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
