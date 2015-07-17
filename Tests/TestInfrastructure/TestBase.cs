using System;
using System.IO;
using EnvDTE;
using Typewriter.CodeModel.CodeDom;
using Typewriter.CodeModel.Providers;
using Xunit;
using File = Typewriter.CodeModel.File;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Typewriter.Tests.TestInfrastructure
{
    //public abstract class CodeDomTestBase : TestBase
    //{
    //    protected CodeDomTestBase()
    //    {
    //        //if (codeModelProvider == null) codeModelProvider = new CodeDomCodeModelProvider();
    //    }
    //}

    //public abstract class RoslynTestBase : TestBase
    //{
    //    protected RoslynTestBase()
    //    {
    //        if (codeModelProvider == null) codeModelProvider = new RoslynProviderStub();
    //    }
    //}

    public abstract class TestBase<T> : IDisposable where T : ICodeModelProvider, new()
    {
        private static DTE dte;
        protected static ICodeModelProvider codeModelProvider;// = new CodeDomCodeModelProvider();

        protected TestBase()
        {
            if(dte == null) dte = Dte.GetInstance("Typewriter.sln");
            if (codeModelProvider == null) codeModelProvider = new T();
            
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
            return codeModelProvider.GetFile(GetProjectItem(path));
        }

        public void Dispose()
        {
            MessageFilter.Revoke();
        }
    }
}