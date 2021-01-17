using System;
using System.Linq;
using Should;
using Typewriter.CodeModel;
using Typewriter.Tests.TestInfrastructure;
using Xunit;
using Typewriter.CodeModel.Configuration;
using Typewriter.Configuration;
using Typewriter.Tests.Metadata.Support;

namespace Typewriter.Tests.Metadata.Roslyn
{
    [Trait("CodeModel", "PartialClasses"), Collection(nameof(RoslynFixture))]
    public class RoslynClassMetadataTests : TestBase
    {
        public RoslynClassMetadataTests(RoslynFixture fixture) : base(fixture)
        {
        }

        private File GetFile(PartialRenderingMode partialRenderingMode)
        {
            var settings = new SettingsImpl(null) { PartialRenderingMode = partialRenderingMode };
            return GetFile(@"Tests\CodeModel\Support\GeneratedClass.cs", settings);
        }

        [Fact]
        public void Expect_all_properties_to_exist()
        {
            var fileInfo = GetFile(PartialRenderingMode.Combined);
            var classInfo = fileInfo.Classes.First();
            var properties = typeof(GeneratedClass).GetProperties();

            classInfo.Properties.All(op => properties.Any(cp => op.Name == cp.Name))
                .ShouldBeTrue();
        }

        [Fact]
        public void Expect_property_attributes_to_exist()
        {
            var fileInfo = GetFile(PartialRenderingMode.Combined);
            var classInfo = fileInfo.Classes.First();

            classInfo.Properties.Any(propertyInfo =>
                propertyInfo.Attributes.Any(a => a.Name == "Key")
            ).ShouldBeTrue();

            var hasDisplayProperty = classInfo.Properties.Any(propertyInfo =>
                propertyInfo.Attributes
                    .Any(a =>
                        a.Name == "Display"
                        && a.Arguments.Any(arg =>
                            string.Equals(
                                arg.Value.ToString(), 
                                "NewPropertyName",
                                StringComparison.InvariantCultureIgnoreCase
                            )
                        )
                    )
            );
            hasDisplayProperty.ShouldBeTrue();
        }
    }
}