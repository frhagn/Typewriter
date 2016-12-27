using System.Linq;
using Should;
using Typewriter.CodeModel;
using Typewriter.Tests.TestInfrastructure;
using Xunit;
using Typewriter.CodeModel.Configuration;
using Typewriter.Configuration;

namespace Typewriter.Tests.CodeModel
{
    [Trait("CodeModel", "PartialClasses"), Collection(nameof(RoslynFixture))]
    public class RoslynPartialClassTests : TestBase
    {
        public RoslynPartialClassTests(RoslynFixture fixture) : base(fixture)
        {
        }

        private File GetFile(PartialRenderingMode partialRenderingMode)
        {
            var settings = new SettingsImpl(null) { PartialRenderingMode = partialRenderingMode };
            return GetFile(@"Tests\CodeModel\Support\PartialClassInfo.cs", settings);
        }

        [Fact]
        public void Expect_name_to_match_class_name()
        {
            var fileInfo = GetFile(PartialRenderingMode.Partial);
            var classInfo = fileInfo.Classes.First();

            classInfo.Name.ShouldEqual("PartialClassInfo");
            classInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.PartialClassInfo");
            classInfo.Namespace.ShouldEqual("Typewriter.Tests.CodeModel.Support");
            classInfo.Parent.ShouldEqual(fileInfo);
        }

        [Fact]
        public void Expect_to_find_doc_comment()
        {
            var fileInfo = GetFile(PartialRenderingMode.Partial);
            var classInfo = fileInfo.Classes.First();
            classInfo.DocComment.Summary.ShouldEqual("summary");
        }

        [Fact]
        public void Expect_to_find_attributes()
        {
            var fileInfo = GetFile(PartialRenderingMode.Partial);
            var classInfo = fileInfo.Classes.First();
            var attributeInfo = classInfo.Attributes.First();

            classInfo.Attributes.Count.ShouldEqual(1);
            attributeInfo.Name.ShouldEqual("AttributeInfo");
            attributeInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.AttributeInfoAttribute");
        }

        [Fact]
        public void Expect_to_find_public_constants_partial()
        {
            var fileInfo = GetFile(PartialRenderingMode.Partial);
            var classInfo = fileInfo.Classes.First();
            var constantInfo = classInfo.Constants.First();

            classInfo.Constants.Count.ShouldEqual(1);
            constantInfo.Name.ShouldEqual("Constant1");
        }

        [Fact]
        public void Expect_to_find_public_constants_full()
        {
            var fileInfo = GetFile(PartialRenderingMode.Combined);
            var classInfo = fileInfo.Classes.First();

            classInfo.Constants.Count.ShouldEqual(2);
            classInfo.Constants.Any(c => c.Name == "Constant1").ShouldBeTrue();
            classInfo.Constants.Any(c => c.Name == "Constant2").ShouldBeTrue();
        }

        [Fact]
        public void Expect_to_find_public_delegates_partial()
        {
            var fileInfo = GetFile(PartialRenderingMode.Partial);
            var classInfo = fileInfo.Classes.First();
            var delegateInfo = classInfo.Delegates.First();

            classInfo.Delegates.Count.ShouldEqual(1);
            delegateInfo.Name.ShouldEqual("Delegate1");
        }

        [Fact]
        public void Expect_to_find_public_delegates_full()
        {
            var fileInfo = GetFile(PartialRenderingMode.Combined);
            var classInfo = fileInfo.Classes.First();

            classInfo.Delegates.Count.ShouldEqual(2);
            classInfo.Delegates.Any(c => c.Name == "Delegate1").ShouldBeTrue();
            classInfo.Delegates.Any(c => c.Name == "Delegate2").ShouldBeTrue();
        }

        [Fact]
        public void Expect_to_find_public_events_partial()
        {
            var fileInfo = GetFile(PartialRenderingMode.Partial);
            var classInfo = fileInfo.Classes.First();
            var delegateInfo = classInfo.Events.First();

            classInfo.Events.Count.ShouldEqual(1);
            delegateInfo.Name.ShouldEqual("Event1");
        }

        [Fact]
        public void Expect_to_find_public_events_full()
        {
            var fileInfo = GetFile(PartialRenderingMode.Combined);
            var classInfo = fileInfo.Classes.First();

            classInfo.Events.Count.ShouldEqual(2);
            classInfo.Events.Any(c => c.Name == "Event1").ShouldBeTrue();
            classInfo.Events.Any(c => c.Name == "Event2").ShouldBeTrue();
        }

        [Fact]
        public void Expect_to_find_public_fields_partial()
        {
            var fileInfo = GetFile(PartialRenderingMode.Partial);
            var classInfo = fileInfo.Classes.First();
            var fieldInfo = classInfo.Fields.First();

            classInfo.Fields.Count.ShouldEqual(1);
            fieldInfo.Name.ShouldEqual("Field1");
        }

        [Fact]
        public void Expect_to_find_public_fields_full()
        {
            var fileInfo = GetFile(PartialRenderingMode.Combined);
            var classInfo = fileInfo.Classes.First();

            classInfo.Fields.Count.ShouldEqual(2);
            classInfo.Fields.Any(c => c.Name == "Field1").ShouldBeTrue();
            classInfo.Fields.Any(c => c.Name == "Field2").ShouldBeTrue();
        }

        [Fact]
        public void Expect_to_find_public_methods_partial()
        {
            var fileInfo = GetFile(PartialRenderingMode.Partial);
            var classInfo = fileInfo.Classes.First();
            var methodInfo = classInfo.Methods.First();

            classInfo.Methods.Count.ShouldEqual(1);
            methodInfo.Name.ShouldEqual("Method1");
        }

        [Fact]
        public void Expect_to_find_public_methods_full()
        {
            var fileInfo = GetFile(PartialRenderingMode.Combined);
            var classInfo = fileInfo.Classes.First();

            classInfo.Methods.Count.ShouldEqual(2);
            classInfo.Methods.Any(c => c.Name == "Method1").ShouldBeTrue();
            classInfo.Methods.Any(c => c.Name == "Method2").ShouldBeTrue();
        }

        [Fact]
        public void Expect_to_find_public_properties_partial()
        {
            var fileInfo = GetFile(PartialRenderingMode.Partial);
            var classInfo = fileInfo.Classes.First();
            var propertyInfo = classInfo.Properties.First();

            classInfo.Properties.Count.ShouldEqual(1);
            propertyInfo.Name.ShouldEqual("Property1");
        }

        [Fact]
        public void Expect_to_find_public_properties_full()
        {
            var fileInfo = GetFile(PartialRenderingMode.Combined);
            var classInfo = fileInfo.Classes.First();

            classInfo.Properties.Count.ShouldEqual(2);
            classInfo.Properties.Any(c => c.Name == "Property1").ShouldBeTrue();
            classInfo.Properties.Any(c => c.Name == "Property2").ShouldBeTrue();
        }

        [Fact]
        public void Expect_to_find_nested_public_classes_partial()
        {
            var fileInfo = GetFile(PartialRenderingMode.Partial);
            var classInfo = fileInfo.Classes.First();
            var nestedClassInfo = classInfo.NestedClasses.First();
            var propertyInfo = nestedClassInfo.Properties.First();

            classInfo.NestedClasses.Count.ShouldEqual(1);
            nestedClassInfo.Name.ShouldEqual("NestedClassInfo1");

            nestedClassInfo.Properties.Count.ShouldEqual(1);
            propertyInfo.Name.ShouldEqual("NestedProperty1");
        }

        [Fact]
        public void Expect_to_find_nested_public_classes_full()
        {
            var fileInfo = GetFile(PartialRenderingMode.Combined);
            var classInfo = fileInfo.Classes.First();

            classInfo.NestedClasses.Count.ShouldEqual(2);
            classInfo.NestedClasses.Any(c => c.Name == "NestedClassInfo1").ShouldBeTrue();
            classInfo.NestedClasses.Any(c => c.Name == "NestedClassInfo2").ShouldBeTrue();
        }

        [Fact]
        public void Expect_to_find_nested_public_enums_partial()
        {
            var fileInfo = GetFile(PartialRenderingMode.Partial);
            var classInfo = fileInfo.Classes.First();
            var nestedEnumInfo = classInfo.NestedEnums.First();
            var valueInfo = nestedEnumInfo.Values.First();

            classInfo.NestedEnums.Count.ShouldEqual(1);
            nestedEnumInfo.Name.ShouldEqual("NestedEnumInfo1");

            nestedEnumInfo.Values.Count.ShouldEqual(1);
            valueInfo.Name.ShouldEqual("NestedValue1");
        }

        [Fact]
        public void Expect_to_find_nested_public_enums_full()
        {
            var fileInfo = GetFile(PartialRenderingMode.Combined);
            var classInfo = fileInfo.Classes.First();

            classInfo.NestedEnums.Count.ShouldEqual(2);
            classInfo.NestedEnums.Any(c => c.Name == "NestedEnumInfo1").ShouldBeTrue();
            classInfo.NestedEnums.Any(c => c.Name == "NestedEnumInfo2").ShouldBeTrue();
        }

        [Fact]
        public void Expect_to_find_nested_public_interfaces_partial()
        {
            var fileInfo = GetFile(PartialRenderingMode.Partial);
            var classInfo = fileInfo.Classes.First();
            var nestedInterfaceInfo = classInfo.NestedInterfaces.First();
            var propertyInfo = nestedInterfaceInfo.Properties.First();

            classInfo.NestedInterfaces.Count.ShouldEqual(1);
            nestedInterfaceInfo.Name.ShouldEqual("INestedInterfaceInfo1");

            nestedInterfaceInfo.Properties.Count.ShouldEqual(1);
            propertyInfo.Name.ShouldEqual("NestedProperty1");
        }

        [Fact]
        public void Expect_to_find_nested_public_interfaces_full()
        {
            var fileInfo = GetFile(PartialRenderingMode.Combined);
            var classInfo = fileInfo.Classes.First();

            classInfo.NestedInterfaces.Count.ShouldEqual(2);
            classInfo.NestedInterfaces.Any(c => c.Name == "INestedInterfaceInfo1").ShouldBeTrue();
            classInfo.NestedInterfaces.Any(c => c.Name == "INestedInterfaceInfo2").ShouldBeTrue();
        }
    }
}