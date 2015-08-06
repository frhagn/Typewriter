using System.Linq;
using EnvDTE;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Typewriter.Metadata.Interfaces;
using Typewriter.Metadata.Providers;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynMetadataProvider : IMetadataProvider
    {
        private readonly Workspace workspace;

        public RoslynMetadataProvider()
        {
            var componentModel = ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel)) as IComponentModel;
            this.workspace = componentModel?.GetService<VisualStudioWorkspace>();
        }

        public IFileMetadata GetFile(ProjectItem projectItem)
        {
            var document = workspace.CurrentSolution.GetDocumentIdsWithFilePath(projectItem.FileNames[1]).FirstOrDefault();
            return new RoslynFileMetadata(workspace.CurrentSolution.GetDocument(document));
        }
    }
}
