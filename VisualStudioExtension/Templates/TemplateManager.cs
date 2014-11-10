using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Typewriter.VisualStudio;

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

        private bool solutionOpen;
        private List<Template> templates;

        public TemplateManager(ILog log, DTE dte, ISolutionMonitor solutionMonitor)
        {
            this.log = log;
            this.dte = dte;

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
                LoadTemplates();
                ValidateLoadedTemplates();

                return this.templates;
            }
        }

        private void LoadTemplates()
        {
            if (this.templates == null)
            {
                var items = GetProjectItems();
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
                    this.templates = null;
                    LoadTemplates();
                    break;
                }
            }
        }

        private IEnumerable<ProjectItem> GetProjectItems()
        {
            foreach (var project in dte.Solution.AllProjetcs())
            {
                foreach (var item in project.AllProjectItems())
                {
                    if (item.Name.EndsWith(templateExtension, StringComparison.InvariantCultureIgnoreCase))
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
