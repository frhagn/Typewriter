using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using EnvDTE;
using Typewriter.CodeModel.CodeDom;
using Typewriter.CodeModel.Providers;
using Typewriter.VisualStudio;

namespace Typewriter.Generation.Controllers
{
    public class GenerationController
    {
        private readonly DTE dte;
        private readonly ICodeModelProvider codeModelProvider;
        private readonly TemplateController templateController;

        public GenerationController(DTE dte, ICodeModelProvider codeModelProvider, SolutionMonitor solutionMonitor, TemplateController templateController, EventQueue eventQueue)
        {
            this.dte = dte;
            this.codeModelProvider = codeModelProvider;
            this.templateController = templateController;

            solutionMonitor.FileAdded += (sender, args) => eventQueue.Enqueue(Render, GenerationType.Render, args.Path);
            solutionMonitor.FileChanged += (sender, args) => eventQueue.Enqueue(Render, GenerationType.Render, args.Path);
            solutionMonitor.FileDeleted += (sender, args) => eventQueue.Enqueue(Render, GenerationType.Delete, args.Path);
            solutionMonitor.FileRenamed += (sender, args) => eventQueue.Enqueue(Render, GenerationType.Rename, args.OldPath, args.NewPath);

            //try
            //{
            //    //var assembly = Assembly.LoadFrom(@"C:\Dev\Typewriter\Typewriter\Workspace\bin\Debug\Typewriter.CodeModel.Workspace.dll");
            //    var assembly = Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName(typeof(GenerationController).Assembly.Location), "Resources", "Typewriter.CodeModel.Workspace.dll"));
            //    var test = assembly.GetType("Typewriter.CodeModel.Workspace.Test");
            //    var create = test.GetMethod("Create");
            //    x = Activator.CreateInstance(test);
            //    create.Invoke(x, new object[] { package });
            //}
            //catch (Exception e)
            //{
            //    var a = 1;
            //}
        }

        private void Render(GenerationEvent generationEvent)
        {
            try
            {
                //var projectItem = dte.Solution.FindProjectItem(generationEvent.Paths[0]);
                //var go = x.GetType().GetMethod("Go");
                //var f = go.Invoke(x, new object[] { projectItem }) as CodeModel.File;

                //if (f != null)
                //{
                //    Log.Debug(f.Name);
                //    Log.Debug(f.FullName);
                //}
                //else
                //    Log.Debug("Fail");

                var templates = templateController.Templates;
                if (templates.Any() == false) return;

                var stopwatch = Stopwatch.StartNew();
                Log.Debug("{0} {1}", generationEvent.Type, string.Join(" -> ", generationEvent.Paths));

                switch (generationEvent.Type)
                {
                    case GenerationType.Render:
                        var file = codeModelProvider.GetFile(dte.Solution.FindProjectItem(generationEvent.Paths[0]));

                        Log.Debug("Roslyn: " + file.Name);

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