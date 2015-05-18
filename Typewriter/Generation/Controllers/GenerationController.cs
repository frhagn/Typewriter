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

        public GenerationController(DTE dte, SolutionMonitor solutionMonitor, TemplateController templateController, EventQueue eventQueue)
        {
            this.dte = dte;
            this.templateController = templateController;

            solutionMonitor.FileAdded += (sender, args) => eventQueue.Enqueue(Render, GenerationType.Render, args.Path);
            solutionMonitor.FileChanged += (sender, args) => eventQueue.Enqueue(Render, GenerationType.Render, args.Path);
            solutionMonitor.FileDeleted += (sender, args) => eventQueue.Enqueue(Render, GenerationType.Delete, args.Path);
            solutionMonitor.FileRenamed += (sender, args) => eventQueue.Enqueue(Render, GenerationType.Rename, args.OldPath, args.NewPath);
        }

        private void Render(GenerationEvent generationEvent)
        {
            try
            {
                var templates = templateController.Templates;
                if (templates.Any() == false) return;

                var stopwatch = Stopwatch.StartNew();
                Log.Debug("{0} {1}", generationEvent.Type, string.Join(" -> ", generationEvent.Paths));

                switch (generationEvent.Type)
                {
                    case GenerationType.Render:
                        var file = new FileInfo(dte.Solution.FindProjectItem(generationEvent.Paths[0]));
                        foreach (var template in templates)
                        {
                            template.RenderFile(file, true);
                        }
                        break;

                    case GenerationType.Delete:
                        foreach (var template in templates)
                        {
                            template.DeleteFile(generationEvent.Paths[0], true);
                        }
                        break;

                    case GenerationType.Rename:
                        foreach (var template in templates)
                        {
                            template.RenameFile(generationEvent.Paths[0], generationEvent.Paths[1], true);
                        }
                        break;
                }

                stopwatch.Stop();
                Log.Debug("{0} completed in {1} ms", generationEvent.Type, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception exception)
            {
                Log.Error("{0} Exception: {1}, {2}", generationEvent.Type, exception.Message, exception.StackTrace);
            }
        }
    }
}