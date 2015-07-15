using System;
using System.Linq;
using EnvDTE;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.LanguageServices;

namespace Typewriter.CodeModel.Workspace
{
    public class Test
    {
        private VisualStudioWorkspace workspace;

        public void Create(object x)
        {
            var method = x.GetType().GetMethod("GetWorkspace");
            var generic = method.MakeGenericMethod(typeof(VisualStudioWorkspace));
            workspace = generic.Invoke(x, null) as VisualStudioWorkspace;

            //var log = x.GetType().GetMethod("Debug");

            //if (workspace != null)
            //{
            //    workspace.WorkspaceChanged += (sender, args) =>
            //    {
            //        log.Invoke(x, new object[] { "ws " + args.Kind.ToString() });

            //        if (args.Kind == WorkspaceChangeKind.SolutionAdded)
            //        {
            //            foreach (var p in workspace.CurrentSolution.Projects)
            //            {
            //                log.Invoke(x, new object[] { p.Name });
            //            }
            //        }
            //    };

            //}

            //var project = workspace.CurrentSolution.Projects.FirstOrDefault(p => p.FilePath == "proj");
            //var document = project.Documents.FirstOrDefault(d => d.FilePath == "path");

            
        }

        public File Go(ProjectItem x)
        {
            var project = workspace.CurrentSolution.Projects.FirstOrDefault(p => p.FilePath == x.ContainingProject.FullName);
            var document = project?.Documents.FirstOrDefault(d => d.FilePath == x.FileNames[1]);

            return document != null ? new FileImpl(document) : null;
        }
    }
}
