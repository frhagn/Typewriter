using System;
using System.IO;
using EnvDTE;
using Typewriter.CodeModel.CodeDom;
using Xunit;
using File = Typewriter.CodeModel.File;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Typewriter.Tests.TestInfrastructure
{
    public abstract class TestBase : IDisposable
    {
        private static readonly DTE dte;

        static TestBase()
        {
            dte = Dte.GetInstance("Typewriter.sln");
        }

        protected TestBase()
        {
            // Handle threading errors when calling into Visual Studio.
            MessageFilter.Register();
        }
        
        protected static string SolutionDirectory => new FileInfo(dte.Solution.FileName).Directory?.FullName;

        protected static ProjectItem GetProjectItem(string path)
        {
            return dte.Solution.FindProjectItem(Path.Combine(SolutionDirectory, path));
        }

        protected static string GetFileContents(string path)
        {
            return System.IO.File.ReadAllText(Path.Combine(SolutionDirectory, path));
        }

        protected static File GetFile(string path)
        {
            return new CodeDomFile(GetProjectItem(path));
        }

        public void Dispose()
        {
            MessageFilter.Revoke();
        }
    }
}