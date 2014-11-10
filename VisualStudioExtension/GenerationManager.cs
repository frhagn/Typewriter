using System;
using System.Linq;
using EnvDTE;
using Typewriter.Templates;
using Typewriter.VisualStudio;
using Typewriter.CodeModel.CodeDom;

namespace Typewriter
{
    public class GenerationManager
    {
        private readonly ILog log;
        private readonly DTE dte;
        private readonly ITemplateManager templateManager;

        public GenerationManager(ILog log, DTE dte, ISolutionMonitor solutionMonitor, ITemplateManager templateManager, IEventQueue eventQueue)
        {
            this.log = log;
            this.dte = dte;
            this.templateManager = templateManager;

            solutionMonitor.FileAdded += (sender, args) => eventQueue.QueueRender(args.Path, Render);
            solutionMonitor.FileChanged += (sender, args) => eventQueue.QueueRender(args.Path, Render);
            solutionMonitor.FileDeleted += (sender, args) => eventQueue.QueueDelete(args.Path, Delete);
            solutionMonitor.FileRenamed += (sender, args) => eventQueue.QueueRename(args.OldPath, args.NewPath, Rename);
        }

        private void Render(string path)
        {
            try
            {
                log.Debug("Render {0}", path);
                if (templateManager.Templates.Any() == false) return;

                var item = dte.Solution.FindProjectItem(path);
                using (var file = new FileInfo(log, item))
                {
                    foreach (var template in templateManager.Templates)
                    {
                        template.Render(file);
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error("Render Exception: {0}", exception.Message);
            }
        }

        private void Delete(string path)
        {
            try
            {
                log.Debug("Delete {0}", path);
                if (templateManager.Templates.Any() == false) return;

                foreach (var template in templateManager.Templates)
                {
                    template.DeleteFile(path);
                }
            }
            catch (Exception exception)
            {
                log.Error("File Deleted Exception: {0}", exception.Message);
            }
        }

        private void Rename(string oldPath, string newPath)
        {
            try
            {
                log.Debug("Rename {0} -> {1}", oldPath, newPath);
                if (templateManager.Templates.Any() == false) return;

                foreach (var template in templateManager.Templates)
                {
                    template.RenameFile(oldPath, newPath);
                }
            }
            catch (Exception exception)
            {
                log.Error("File Renamed Exception: {0}", exception.Message);
            }
        }
    }
}