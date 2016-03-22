using System.Linq;
using Should;
using Typewriter.CodeModel;
using Typewriter.Tests.TestInfrastructure;
using Xunit;

namespace Typewriter.Tests.CodeModel
{
    [Trait("CodeModel", "Enums"), Collection(nameof(CodeDomFixture))]
    public class CodeDomEnumTests : EnumTests
    {
        public CodeDomEnumTests(CodeDomFixture fixture) : base(fixture)
        {
        }
    }

    [Trait("CodeModel", "Enums"), Collection(nameof(RoslynFixture))]
    public class RoslynEnumTests : EnumTests
    {
        public RoslynEnumTests(RoslynFixture fixture) : base(fixture)
        {
        }
    }

    public abstract class EnumTests : TestBase
    {
        private readonly File fileInfo;

        protected EnumTests(ITestFixture fixture) : base(fixture)
        {
            fileInfo = GetFile(@"Tests\CodeModel\Support\EnumInfo.cs");
        }

        [Fact]
        public void Expect_name_to_match_enum_name()
        {
            var enumInfo = fileInfo.Enums.First();

            enumInfo.Name.ShouldEqual("EnumInfo");
            enumInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.EnumInfo");
            enumInfo.Namespace.ShouldEqual("Typewriter.Tests.CodeModel.Support");
            enumInfo.Parent.ShouldEqual(fileInfo);
        }

        [Fact]
        public void Expect_to_find_doc_comment()
        {
            var enumInfo = fileInfo.Enums.First();
            enumInfo.DocComment.Summary.ShouldEqual("summary");
        }

        [Fact]
        public void Expect_to_find_attributes()
        {
            var enumInfo = fileInfo.Enums.First();
            var attributeInfo = enumInfo.Attributes.First();

            enumInfo.Attributes.Count.ShouldEqual(1);
            attributeInfo.Name.ShouldEqual("AttributeInfo");
            attributeInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.AttributeInfoAttribute");
        }

        [Fact]
        public void Expect_to_find_value_attributes()
        {
            var enumInfo = fileInfo.Enums.First();
            var valueInfo = enumInfo.Values.First();
            var attributeInfo = valueInfo.Attributes.First();

            enumInfo.Attributes.Count.ShouldEqual(1);
            attributeInfo.Name.ShouldEqual("AttributeInfo");
            attributeInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.AttributeInfoAttribute");
        }

        [Fact]
        public void Expect_unspecified_values_to_count_from_zero()
        {
            var enumInfo = fileInfo.Enums.First();
            var firstValue = enumInfo.Values.First(v => v.Name == "ValueA");
            var secondValue = enumInfo.Values.First(v => v.Name == "ValueB");

            firstValue.Value.ShouldEqual(0);
            secondValue.Value.ShouldEqual(1);
        }

        [Fact]
        public void Expect_values_after_a_specified_values_to_count_from_the_prevoius_value()
        {
            var enumInfo = fileInfo.Enums.First();
            var thirdValue = enumInfo.Values.First(v => v.Name == "ValueC");
            var fourthValue = enumInfo.Values.First(v => v.Name == "ValueD");

            thirdValue.Value.ShouldEqual(5);
            fourthValue.Value.ShouldEqual(6);
        }

        [Fact]
        public void Expect_values_specified_with_a_char_to_be_converted_to_their_integer_value()
        {
            var enumInfo = fileInfo.Enums.First();
            var fifthValue = enumInfo.Values.First(v => v.Name == "ValueE");
            var sixthValue = enumInfo.Values.First(v => v.Name == "ValueF");

            fifthValue.Value.ShouldEqual('A'); // A = 65
            fifthValue.Value.ShouldEqual(65);
            sixthValue.Value.ShouldEqual(66);
        }

        [Fact]
        public void Expect_enums_marked_with_flags_attribute_to_be_flags()
        {
            var flagsEnumInfo = fileInfo.Enums.First(e => e.Name == "FlagsEnumInfo");
            flagsEnumInfo.IsFlags.ShouldBeTrue();
        }

        [Fact]
        public void Expect_enums_not_marked_with_flags_attribute_to_be_flags()
        {
            var enumInfo = fileInfo.Enums.First();
            enumInfo.IsFlags.ShouldBeFalse();
        }

        [Fact]
        public void Expect_hexadecimal_values_converted_to_their_integer_value()
        {
            var hexEnumInfo = fileInfo.Enums.First(e => e.Name == "HexEnumInfo");
            var firstValue = hexEnumInfo.Values.First(v => v.Name == "ValueA");
            var thirdValue = hexEnumInfo.Values.First(v => v.Name == "ValueC");

            firstValue.Value.ShouldEqual(1);
            firstValue.Value.ShouldEqual(0x01);

            thirdValue.Value.ShouldEqual(3);
            thirdValue.Value.ShouldEqual(0x03);
        }

        [Fact]
        public void Expect_to_find_containing_class_on_nested_enum()
        {
            var classInfo = fileInfo.Classes.First();
            var nestedEnumInfo = classInfo.NestedEnums.First();
            var containingClassInfo = nestedEnumInfo.ContainingClass;

            containingClassInfo.Name.ShouldEqual("EnumContiningClassInfo");
        }

        [Fact]
        public void Expect_not_to_find_containing_class_on_top_level_enum()
        {
            var enumInfo = fileInfo.Enums.First();
            var containingClassInfo = enumInfo.ContainingClass;

            containingClassInfo.ShouldBeNull();
        }
    }
}
