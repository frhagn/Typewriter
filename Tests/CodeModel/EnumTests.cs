using System.Linq;
using Should;
using Tests.CodeModel.Support;
using Tests.TestInfrastructure;
using Typewriter.CodeModel;
using Xunit;

namespace Tests.CodeModel
{
    [Trait("CodeModel", "Enums")]
    public class EnumTests : TestBase
    {
        private readonly File fileInfo = GetFile(@"Tests\CodeModel\Support\EnumInfo.cs");

        [Fact]
        public void Expect_name_to_match_enum_name()
        {
            var enumInfo = fileInfo.Enums.First();

            enumInfo.Name.ShouldEqual("EnumInfo");
            enumInfo.FullName.ShouldEqual("Tests.CodeModel.Support.EnumInfo");
            enumInfo.Namespace.ShouldEqual("Tests.CodeModel.Support");
            enumInfo.Parent.ShouldEqual(fileInfo);
        }

        [Fact]
        public void Expect_to_find_attributes()
        {
            var enumInfo = fileInfo.Enums.First();
            var attributeInfo = enumInfo.Attributes.First();

            enumInfo.Attributes.Count.ShouldEqual(1);
            attributeInfo.Name.ShouldEqual("AttributeInfo");
        }

        [Fact]
        public void Expect_to_find_value_attributes()
        {
            var enumInfo = fileInfo.Enums.First();
            var valueInfo = enumInfo.Values.First();
            var attributeInfo = valueInfo.Attributes.First();

            enumInfo.Attributes.Count.ShouldEqual(1);
            attributeInfo.Name.ShouldEqual("AttributeInfo");
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
    }
}
