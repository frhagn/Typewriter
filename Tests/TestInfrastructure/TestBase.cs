using System.IO;
using EnvDTE;
using Typewriter.CodeModel.Implementation;
using Typewriter.Metadata.Providers;
using Xunit;
using File = Typewriter.CodeModel.File;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Typewriter.Tests.TestInfrastructure
{
    public abstract class TestBase
    {
        private readonly DTE dte;
        private readonly IMetadataProvider metadataProvider;

        protected TestBase(ITestFixture fixture)
        {
            this.dte = fixture.Dte;
            this.metadataProvider = fixture.Provider;
        }
        
        protected string SolutionDirectory => new FileInfo(dte.Solution.FileName).Directory?.FullName;

        protected ProjectItem GetProjectItem(string path)
        {
            return dte.Solution.FindProjectItem(Path.Combine(SolutionDirectory, path));
        }

        protected string GetFileContents(string path)
        {
            return System.IO.File.ReadAllText(Path.Combine(SolutionDirectory, path));
        }

        protected File GetFile(string path)
        {
            var metadata = metadataProvider.GetFile(Path.Combine(SolutionDirectory, path));
            return new FileImpl(metadata);
        }
    }
}