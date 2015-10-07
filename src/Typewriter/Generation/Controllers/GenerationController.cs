using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
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
        private readonly EventQueue eventQueue;

        public GenerationController(DTE dte, IMetadataProvider metadataProvider, SolutionMonitor solutionMonitor, TemplateController templateController, EventQueue eventQueue)
        {
            this.dte = dte;
            this.metadataProvider = metadataProvider;
            this.templateController = templateController;
            this.eventQueue = eventQueue;

            solutionMonitor.FileAdded += (sender, args) => Process(GenerationType.Render, args.Path);
            solutionMonitor.FileChanged += (sender, args) => Process(GenerationType.Render, args.Path);
            solutionMonitor.FileDeleted += (sender, args) => Process( GenerationType.Delete, args.Path);
            solutionMonitor.FileRenamed += (sender, args) => Process( GenerationType.Rename, args.OldPath, args.NewPath);
        }

        private void Process(GenerationType type, params string[] paths)
        {
            if (paths[0].EndsWith(".cs", StringComparison.InvariantCultureIgnoreCase) == false) return;

            eventQueue.Enqueue(Render, type, paths);

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
                        var metadata = metadataProvider.GetFile(generationEvent.Paths[0]);
                        var file = new FileImpl(metadata);

                        foreach (var template in templates)
                        {
                            template.RenderFile(file);
                        }
                        break;

                    case GenerationType.Delete:
                        foreach (var template in templates)
                        {
                            template.DeleteFile(generationEvent.Paths[0]);
                        }
                        break;

                    case GenerationType.Rename:
                        foreach (var template in templates)
                        {
                            template.RenameFile(generationEvent.Paths[0], generationEvent.Paths[1]);
                        }
                        break;
                }

                foreach (var template in templates)
                {
                    template.SaveProjectFile();
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