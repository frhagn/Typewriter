using System.Linq;
using EnvDTE;
using Microsoft.CodeAnalysis.MSBuild;
using Typewriter.Metadata.Interfaces;
using Typewriter.Metadata.Providers;
using Typewriter.Metadata.Roslyn;
using Typewriter.Configuration;
using System;

namespace Typewriter.Tests.TestInfrastructure
{
    public class RoslynMetadataProviderStub : IMetadataProvider
    {
        private readonly Microsoft.CodeAnalysis.Workspace workspace;

        public RoslynMetadataProviderStub(DTE dte)
        {
            var solutionPath = dte.Solution.FullName;
            var msBuildWorkspace = MSBuildWorkspace.Create();
            msBuildWorkspace.OpenSolutionAsync(solutionPath).GetAwaiter().GetResult();
            
            // ReSharper disable once UnusedVariable
            //ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            //{
            //    await msBuildWorkspace.OpenSolutionAsync(solutionPath);
            //}).Join();

            this.workspace = msBuildWorkspace;
        }

        public IFileMetadata GetFile(string path, Settings settings, Action<string[]> requestRender)
        {
            var document = workspace.CurrentSolution.GetDocumentIdsWithFilePath(path).FirstOrDefault();
            if (document != null)
            {
                return new RoslynFileMetadata(workspace.CurrentSolution.GetDocument(document), settings, requestRender);
            }

            return null;
        }
    }
}
