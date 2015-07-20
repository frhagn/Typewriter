using System;
using System.IO;
using EnvDTE;
using Typewriter.CodeModel.Implementation;
using Typewriter.Metadata.Providers;
using Xunit;
using File = Typewriter.CodeModel.File;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Typewriter.Tests.TestInfrastructure
{
    //public abstract class CodeDomTestBase : TestBase
    //{
    //    protected CodeDomTestBase()
    //    {
    //        //if (codeModelProvider == null) codeModelProvider = new CodeDomMetadataProvider();
    //    }
    //}

    //public abstract class RoslynTestBase : TestBase
    //{
    //    protected RoslynTestBase()
    //    {
    //        if (codeModelProvider == null) codeModelProvider = new RoslynProviderStub();
    //    }
    //}

    public abstract class TestBase<T> : IDisposable where T : IMetadataProvider, new()
    {
        private static DTE dte;
        protected static IMetadataProvider metadataProvider;// = new CodeDomMetadataProvider();

        protected TestBase()
        {
            if(dte == null) dte = Dte.GetInstance("Typewriter.sln");
            if (metadataProvider == null) metadataProvider = new T();
            
            // Handle threading errors when calling into Visual Studio.
            MessageFilter.Register();
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
            var metadata = metadataProvider.GetFile(GetProjectItem(path));
            return new FileImpl(metadata);
        }

        public void Dispose()
        {
            MessageFilter.Revoke();
        }
    }
}