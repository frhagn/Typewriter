using System;
using System.Diagnostics;
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
                var stopwatch = Stopwatch.StartNew();

                var templates = templateManager.Templates;
                if (templates.Any())
                {
                    var item = dte.Solution.FindProjectItem(path);
                    var file = new FileInfo(log, item);

                    foreach (var template in templates)
                    {
                        template.Render(file);
                    }
                }

                stopwatch.Stop();
                log.Debug("Render completed in {0} ms", stopwatch.ElapsedMilliseconds);
            }
            catch (Exception exception)
            {
                log.Error("Render Exception: {0}, {1}", exception.Message, exception.StackTrace);
            }
        }

        private void Delete(string path)
        {
            try
            {
                var templates = templateManager.Templates;
                if (templates.Any() == false) return;

                log.Debug("Delete {0}", path);
                var stopwatch = Stopwatch.StartNew();

                foreach (var template in templates)
                {
                    template.DeleteFile(path);
                }

                stopwatch.Stop();
                log.Debug("Delete completed in {0} ms", stopwatch.ElapsedMilliseconds);
            }
            catch (Exception exception)
            {
                log.Error("File Deleted Exception: {0}, {1}", exception.Message, exception.StackTrace);
            }
        }

        private void Rename(string oldPath, string newPath)
        {
            try
            {
                var templates = templateManager.Templates;
                if (templates.Any() == false) return;

                log.Debug("Rename {0} -> {1}", oldPath, newPath);
                var stopwatch = Stopwatch.StartNew();

                foreach (var template in templates)
                {
                    template.RenameFile(oldPath, newPath);
                }

                stopwatch.Stop();
                log.Debug("Rename completed in {0} ms", stopwatch.ElapsedMilliseconds);
            }
            catch (Exception exception)
            {
                log.Error("File Renamed Exception: {0}, {1}", exception.Message, exception.StackTrace);
            }
        }
    }
}