using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Typewriter.Metadata.Interfaces;
using Typewriter.Metadata.Providers;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynMetadataProvider : IMetadataProvider
    {
        private readonly Microsoft.CodeAnalysis.Workspace workspace;

        public RoslynMetadataProvider()
        {
            var componentModel = ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel)) as IComponentModel;
            this.workspace = componentModel.GetService<VisualStudioWorkspace>();
        }

        public IFileMetadata GetFile(ProjectItem projectItem)
        {
            var project = workspace.CurrentSolution.Projects.FirstOrDefault(p => p.FilePath == projectItem.ContainingProject.FullName);
            var document = project?.Documents.FirstOrDefault(d => d.FilePath == projectItem.FileNames[1]);

            return new RoslynFileMetadata(document);
        }
    }
}
