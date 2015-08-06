using System.Linq;
using EnvDTE;
using Microsoft.CodeAnalysis.MSBuild;
using Typewriter.Metadata.Interfaces;
using Typewriter.Metadata.Providers;
using Typewriter.Metadata.Roslyn;

namespace Typewriter.Tests.TestInfrastructure
{
    public class RoslynMetadataProviderStub : IMetadataProvider
    {
        private readonly Microsoft.CodeAnalysis.Workspace workspace;

        public RoslynMetadataProviderStub(DTE dte)
        {
            var solutionPath = dte.Solution.FullName;
            var msBuildWorkspace = MSBuildWorkspace.Create();

            // ReSharper disable once UnusedVariable
            var solution = msBuildWorkspace.OpenSolutionAsync(solutionPath).Result;

            this.workspace = msBuildWorkspace;
        }

        public IFileMetadata GetFile(ProjectItem projectItem)
        {
            var document = workspace.CurrentSolution.GetDocumentIdsWithFilePath(projectItem.FileNames[1]).FirstOrDefault();
            return new RoslynFileMetadata(workspace.CurrentSolution.GetDocument(document));
        }
    }
}
