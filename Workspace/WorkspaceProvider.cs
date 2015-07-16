using System;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Typewriter.CodeModel.Providers;

namespace Typewriter.CodeModel.Workspace
{
    public class WorkspaceProvider : ICodeModelProvider
    {
        private readonly VisualStudioWorkspace workspace;

        public WorkspaceProvider()
        {
            var componentModel = ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel)) as IComponentModel;
            this.workspace = componentModel?.GetService<VisualStudioWorkspace>();
        }

        public File GetFile(ProjectItem projectItem)
        {
            var project = workspace.CurrentSolution.Projects.FirstOrDefault(p => p.FilePath == projectItem.ContainingProject.FullName);
            var document = project?.Documents.FirstOrDefault(d => d.FilePath == projectItem.FileNames[1]);

            return new FileImpl(document);
        }
    }
}
