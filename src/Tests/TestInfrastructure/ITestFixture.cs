using System;
using EnvDTE;
using Typewriter.Metadata.Providers;

namespace Typewriter.Tests.TestInfrastructure
{
    public interface ITestFixture : IDisposable
    {
        DTE Dte { get; }
        IMetadataProvider Provider { get; }
    }
}