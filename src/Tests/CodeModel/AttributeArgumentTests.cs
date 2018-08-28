using System.Linq;
using Should;
using Typewriter.CodeModel;
using Typewriter.Tests.CodeModel.Support;
using Typewriter.Tests.TestInfrastructure;
using Xunit;

namespace Typewriter.Tests.CodeModel
{
    [Trait("CodeModel", "AttributeArguments"), Collection(nameof(RoslynFixture))]
    public class RoslynAttributeArgumentTests : AttributeArgumentTests
    {
        public RoslynAttributeArgumentTests(RoslynFixture fixture) : base(fixture)
        {
        }
    }

    public abstract class AttributeArgumentTests : TestBase
    {
        private readonly Class classInfo;

        protected AttributeArgumentTests(ITestFixture fixture) : base(fixture)
        {
            var fileInfo = GetFile(@"Tests\CodeModel\Support\AttributeInfo.cs");
            classInfo = fileInfo.Classes.First(c => c.Name == nameof(AttributeTestClass));
        }

        [Fact]
        public void Expect_attributes_with_no_parameters_to_have_an_empty_value()
        {
            var propertyInfo = classInfo.Properties.First(p => p.Name == "NoParameters");
            var attributeInfo = propertyInfo.Attributes.First();

            attributeInfo.Arguments.ShouldBeEmpty();
        }

        [Fact]
        public void Expect_attributes_with_string_parameter_to_have_a_string_value()
        {
            var propertyInfo = classInfo.Properties.First(p => p.Name == "StringParameter");
            var attributeInfo = propertyInfo.Attributes.First();

            var attributeArgument = attributeInfo.Arguments.Single();
            attributeArgument.Value.ShouldEqual("parameter");
            attributeArgument.Type.OriginalName.ShouldEqual("string");
        }

        [Fact]
        public void Expect_attributes_with_int_parameter_to_have_an_integer_value()
        {
            var propertyInfo = classInfo.Properties.First(p => p.Name == "IntParameter");
            var attributeInfo = propertyInfo.Attributes.First();

            var attributeArgument = attributeInfo.Arguments.Single();
            attributeArgument.Value.ShouldEqual(1);
            attributeArgument.Type.OriginalName.ShouldEqual("int");
        }

        [Fact]
        public void Expect_attributes_with_int_and_named_parameter_to_have_a_proper_value()
        {
            var propertyInfo = classInfo.Properties.First(p => p.Name == "IntAndNamedParameter");
            var attributeInfo = propertyInfo.Attributes.First();

            var integerArgument = attributeInfo.Arguments.First();
            integerArgument.Value.ShouldEqual(2);
            integerArgument.Type.OriginalName.ShouldEqual("int");

            var stringArgument = attributeInfo.Arguments.Skip(1).First();
            stringArgument.Value.ShouldEqual("parameter");
            stringArgument.Type.OriginalName.ShouldEqual("string");
        }

        [Fact]
        public void Expect_attributes_with_params_parameter_to_have_a_proper_value()
        {
            var propertyInfo = classInfo.Properties.First(p => p.Name == "ParamsParameter");
            var attributeInfo = propertyInfo.Attributes.First();

            var stringArgument1 = attributeInfo.Arguments.First();
            stringArgument1.Value.ShouldEqual(new object[] { "parameter1", "parameter2" });
            stringArgument1.Type.OriginalName.ShouldEqual("string[]");

        }

        [Fact]
        public void Expect_attributes_with_string_and_params_parameter_to_have_a_proper_value()
        {
            var propertyInfo = classInfo.Properties.First(p => p.Name == "IntAndParamsParameter");
            var attributeInfo = propertyInfo.Attributes.First();

            var integerArgument = attributeInfo.Arguments.First();
            integerArgument.Value.ShouldEqual(1);
            integerArgument.Type.OriginalName.ShouldEqual("int");

            var stringArgument = attributeInfo.Arguments.Skip(1).First();
            stringArgument.Value.ShouldEqual(new object[] { "parameter" });
            stringArgument.Type.OriginalName.ShouldEqual("string[]");
        }

        [Fact]
        public void Expect_attributes_with_type_parameter_to_have_a_proper_value()
        {
            var propertyInfo = classInfo.Properties.First(p => p.Name == "Type");
            var attributeInfo = propertyInfo.Attributes.First();

            var integerArgument = attributeInfo.Arguments.Single();
            integerArgument.TypeValue.Name.ShouldEqual("AttributeTestClass");
            integerArgument.Type.OriginalName.ShouldEqual("Type");
        }
    }
}