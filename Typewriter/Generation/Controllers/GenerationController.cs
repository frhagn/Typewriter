using System;
using System.Diagnostics;
using System.Linq;
using EnvDTE;
using Typewriter.CodeModel.CodeDom;
using Typewriter.VisualStudio;

namespace Typewriter.Generation.Controllers
{
    public class GenerationController
    {
        private readonly DTE dte;
        private readonly TemplateController templateController;

        public GenerationController(DTE dte, ISolutionMonitor solutionMonitor, TemplateController templateController, IEventQueue eventQueue)
        {
            this.dte = dte;
            this.templateController = templateController;

            solutionMonitor.FileAdded += (sender, args) => eventQueue.QueueRender(args.Path, Render);
            solutionMonitor.FileChanged += (sender, args) => eventQueue.QueueRender(args.Path, Render);
            solutionMonitor.FileDeleted += (sender, args) => eventQueue.QueueDelete(args.Path, Delete);
            solutionMonitor.FileRenamed += (sender, args) => eventQueue.QueueRename(args.OldPath, args.NewPath, Rename);
        }

        private void Render(string path)
        {
            try
            {
                Log.Debug("Render {0}", path);
                var stopwatch = Stopwatch.StartNew();

                var templates = templateController.Templates;
                if (templates.Any())
                {
                    var item = dte.Solution.FindProjectItem(path);
                    var file = new FileInfo(item);

                    foreach (var template in templates)
                    {
                        template.Render(file);
                    }
                }

                stopwatch.Stop();
                Log.Debug("Render completed in {0} ms", stopwatch.ElapsedMilliseconds);
            }
            catch (Exception exception)
            {
                Log.Error("Render Exception: {0}, {1}", exception.Message, exception.StackTrace);
            }
        }

        private void Delete(string path)
        {
            try
            {
                var templates = templateController.Templates;
                if (templates.Any() == false) return;

                Log.Debug("Delete {0}", path);
                var stopwatch = Stopwatch.StartNew();

                foreach (var template in templates)
                {
                    template.DeleteFile(path);
                }

                stopwatch.Stop();
                Log.Debug("Delete completed in {0} ms", stopwatch.ElapsedMilliseconds);
            }
            catch (Exception exception)
            {
                Log.Error("File Deleted Exception: {0}, {1}", exception.Message, exception.StackTrace);
            }
        }

        private void Rename(string oldPath, string newPath)
        {
            try
            {
                var templates = templateController.Templates;
                if (templates.Any() == false) return;

                Log.Debug("Rename {0} -> {1}", oldPath, newPath);
                var stopwatch = Stopwatch.StartNew();

                foreach (var template in templates)
                {
                    template.RenameFile(oldPath, newPath);
                }

                stopwatch.Stop();
                Log.Debug("Rename completed in {0} ms", stopwatch.ElapsedMilliseconds);
            }
            catch (Exception exception)
            {
                Log.Error("File Renamed Exception: {0}, {1}", exception.Message, exception.StackTrace);
            }
        }
    }
}