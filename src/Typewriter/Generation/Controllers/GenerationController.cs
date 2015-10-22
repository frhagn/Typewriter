using System;
using System.Diagnostics;
using System.Linq;
using EnvDTE;
using Typewriter.CodeModel;
using Typewriter.CodeModel.Implementation;
using Typewriter.Metadata.Providers;
using Typewriter.VisualStudio;

namespace Typewriter.Generation.Controllers
{
    public class GenerationController
    {
        private readonly DTE dte;
        private readonly IMetadataProvider metadataProvider;
        private readonly TemplateController templateController;

        public GenerationController(DTE dte, IMetadataProvider metadataProvider, SolutionMonitor solutionMonitor, TemplateController templateController, EventQueue eventQueue)
        {
            this.dte = dte;
            this.metadataProvider = metadataProvider;
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
                        var includedTemplates = templates.Where(t => t.ShouldRenderFile(generationEvent.Paths[0])).ToList();
                        if (includedTemplates.Any())
                        {
                            var metadata = metadataProvider.GetFile(generationEvent.Paths[0]);
                            var file = new FileImpl(metadata);

                            foreach (var template in includedTemplates)
                            {
                                template.RenderFile(file, true);
                            }
                        }
                        break;

                    case GenerationType.Delete:
                        foreach (var template in templates)
                        {
                            template.DeleteFile(generationEvent.Paths[0], true);
                        }
                        break;

                    case GenerationType.Rename:
                        var rmetadata = metadataProvider.GetFile(generationEvent.Paths[1]);
                        var rfile = new FileImpl(rmetadata);

                        Log.Debug("From: " + generationEvent.Paths[0]);
                        Log.Debug("Till: " + generationEvent.Paths[1]);

                        foreach (var template in templates)
                        {
                            template.RenameFile(rfile, generationEvent.Paths[0], generationEvent.Paths[1], true);
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