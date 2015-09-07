using EnvDTE;
using Typewriter.Metadata.CodeDom;
using Typewriter.Metadata.Providers;
using Xunit;

namespace Typewriter.Tests.TestInfrastructure
{
    public class CodeDomFixture : ITestFixture
    {
        public CodeDomFixture()
        {
            Dte = TestInfrastructure.Dte.GetInstance("Typewriter.sln");
            Provider = new CodeDomMetadataProvider(Dte);

            // Handle threading errors when calling into Visual Studio.
            MessageFilter.Register();
        }

        public DTE Dte { get; }
        public IMetadataProvider Provider { get; }

        public void Dispose()
        {
            MessageFilter.Revoke();
        }
    }

    [CollectionDefinition(nameof(CodeDomFixture))]
    public class CodeDomCollection : ICollectionFixture<CodeDomFixture>
    {
    }
}