using System.Linq;
using Should;
using Typewriter.CodeModel;
using Typewriter.Tests.TestInfrastructure;
using Xunit;

namespace Typewriter.Tests.CodeModel
{
	 [Trait("CodeModel", "Properties"), Collection(nameof(CodeDomFixture))]
	 public class CodeDomPropertyTests : PropertyTests
	 {
		  public CodeDomPropertyTests(CodeDomFixture fixture) : base(fixture)
		  {
		  }
	 }

	 [Trait("CodeModel", "Properties"), Collection(nameof(RoslynFixture))]
	 public class RoslynPropertyTests : PropertyTests
	 {
		  public RoslynPropertyTests(RoslynFixture fixture) : base(fixture)
		  {
		  }
	 }

    public abstract class PropertyTests : TestBase
    {
        private readonly File fileInfo;
        private readonly Class classInfo;

        protected PropertyTests(ITestFixture fixture) : base(fixture)
        {
            fileInfo = GetFile(@"Tests\CodeModel\Support\PropertyInfo.cs");
            classInfo = fileInfo.Classes.First();
        }

        protected Property GetFirstProperty(string name) => classInfo.Properties.First(p => p.Name == name);

        [Fact]
        public void Expect_name_to_match_property_name()
        {
            var propertyInfo = GetFirstProperty("Bool");

            propertyInfo.Name.ShouldEqual("Bool");
            propertyInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.PropertyInfo.Bool");
            propertyInfo.Parent.ShouldEqual(classInfo);
        }

        [Fact]
        public void Expect_to_find_doc_comment()
        {
            var propertyInfo = GetFirstProperty("GetterOnly");
            propertyInfo.DocComment.Summary.ShouldEqual("summary");
        }

        [Fact]
        public void Expect_to_find_attributes()
        {
            var propertyInfo = GetFirstProperty("Bool");
            var attributeInfo = propertyInfo.Attributes.First();

            propertyInfo.Attributes.Count.ShouldEqual(1);
            attributeInfo.Name.ShouldEqual("AttributeInfo");
            attributeInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.AttributeInfoAttribute");
        }

        [Fact]
        public void Expect_properties_without_public_getter_to_only_have_setter()
        {
            var setterOnly = GetFirstProperty("SetterOnly");
            var privateGetter = GetFirstProperty("PrivateGetter");

            setterOnly.HasGetter.ShouldBeFalse();
            setterOnly.HasSetter.ShouldBeTrue();
            privateGetter.HasGetter.ShouldBeFalse();
            privateGetter.HasSetter.ShouldBeTrue();
        }

        [Fact]
        public void Expect_properties_without_public_setter_to_only_have_getter()
        {
            var getterOnly = GetFirstProperty("GetterOnly");
            var privateSetter = GetFirstProperty("PrivateSetter");

            getterOnly.HasGetter.ShouldBeTrue();
            getterOnly.HasSetter.ShouldBeFalse();
            privateSetter.HasGetter.ShouldBeTrue();
            privateSetter.HasSetter.ShouldBeFalse();
        }

        [Fact]
        public void Expect_properties_with_public_getter_and_setter_to_have_both_getter_and_setter()
        {
            var propertyInfo = GetFirstProperty("Bool");

            propertyInfo.HasGetter.ShouldBeTrue();
            propertyInfo.HasSetter.ShouldBeTrue();
        }

        [Fact]
        public void Expect_primitive_properties_to_be_primitive()
        {
            var properties = new[]
            {
                "Bool", "Char", "String", "Byte", "Sbyte", "Int", "Uint", "Short", "Ushort", "Long", "Ulong", "Float",
                "Double", "Decimal"
            };

            foreach (var property in properties)
            {
                var propertyInfo = GetFirstProperty(property);

                propertyInfo.Type.IsEnum.ShouldBeFalse($"IsEnum {property}");
                propertyInfo.Type.IsEnumerable.ShouldBeFalse($"IsEnumerable {property}");
                propertyInfo.Type.IsGeneric.ShouldBeFalse($"IsGeneric {property}");
                propertyInfo.Type.IsNullable.ShouldBeFalse($"IsNullable {property}");
                propertyInfo.Type.IsPrimitive.ShouldBeTrue($"IsPrimitive {property}");
                propertyInfo.Type.IsDate.ShouldBeFalse($"IsDate {property}");
                propertyInfo.Type.IsGuid.ShouldBeFalse($"IsGuid {property}");
                propertyInfo.Type.IsTimeSpan.ShouldBeFalse($"IsTimeSpan {property}");
            }
        }

        [Fact]
        public void Expect_datetime_properties_to_be_primitive()
        {
            var properties = new[] {"DateTime", "DateTimeOffset"};

            foreach (var property in properties)
            {
                var propertyInfo = GetFirstProperty(property);

                propertyInfo.Type.IsEnum.ShouldBeFalse($"IsEnum {property}");
                propertyInfo.Type.IsEnumerable.ShouldBeFalse($"IsEnumerable {property}");
                propertyInfo.Type.IsGeneric.ShouldBeFalse($"IsGeneric {property}");
                propertyInfo.Type.IsNullable.ShouldBeFalse($"IsNullable {property}");
                propertyInfo.Type.IsTask.ShouldBeFalse($"IsTask {property}");
                propertyInfo.Type.IsPrimitive.ShouldBeTrue($"IsPrimitive {property}");
                propertyInfo.Type.IsDate.ShouldBeTrue($"IsDate {property}");
                propertyInfo.Type.IsGuid.ShouldBeFalse($"IsGuid {property}");
                propertyInfo.Type.IsTimeSpan.ShouldBeFalse($"IsTimeSpan {property}");

                propertyInfo.Type.Name.ShouldEqual("Date");
            }
        }

        [Fact]
        public void Expect_guid_properties_to_be_primitive()
        {
            var propertyInfo = GetFirstProperty("Guid");

            propertyInfo.Type.IsEnum.ShouldBeFalse("IsEnum");
            propertyInfo.Type.IsEnumerable.ShouldBeFalse("IsEnumerable");
            propertyInfo.Type.IsGeneric.ShouldBeFalse("IsGeneric");
            propertyInfo.Type.IsNullable.ShouldBeFalse("IsNullable");
            propertyInfo.Type.IsTask.ShouldBeFalse("IsTask");
            propertyInfo.Type.IsPrimitive.ShouldBeTrue("IsPrimitive");
            propertyInfo.Type.IsDate.ShouldBeFalse("IsDate");
            propertyInfo.Type.IsGuid.ShouldBeTrue("IsGuid");
            propertyInfo.Type.IsTimeSpan.ShouldBeFalse("IsTimeSpan");

            propertyInfo.Type.Name.ShouldEqual("string");
        }

        [Fact]
        public void Expect_timespan_properties_to_be_primitive()
        {
            var propertyInfo = GetFirstProperty("TimeSpan");

            propertyInfo.Type.IsEnum.ShouldBeFalse("IsEnum");
            propertyInfo.Type.IsEnumerable.ShouldBeFalse("IsEnumerable");
            propertyInfo.Type.IsGeneric.ShouldBeFalse("IsGeneric");
            propertyInfo.Type.IsNullable.ShouldBeFalse("IsNullable");
            propertyInfo.Type.IsTask.ShouldBeFalse("IsTask");
            propertyInfo.Type.IsPrimitive.ShouldBeTrue("IsPrimitive");
            propertyInfo.Type.IsDate.ShouldBeFalse("IsDate");
            propertyInfo.Type.IsGuid.ShouldBeFalse("IsGuid");
            propertyInfo.Type.IsTimeSpan.ShouldBeTrue("IsTimeSpan");

            propertyInfo.Type.Name.ShouldEqual("string");
        }

        [Fact]
        public void Expect_object_and_dynamic_properties_to_return_any()
        {
            var objectInfo = GetFirstProperty("Object");
            var dynamicInfo = GetFirstProperty("Dynamic");

            objectInfo.Type.Name.ShouldEqual("any");
            objectInfo.Type.FullName.ShouldEqual("System.Object");
            objectInfo.Type.OriginalName.ShouldEqual("Object");
            dynamicInfo.Type.Name.ShouldEqual("any");
            dynamicInfo.Type.FullName.ShouldEqual("dynamic");
            dynamicInfo.Type.OriginalName.ShouldEqual("dynamic");
        }

        [Fact]
        public void Expect_enum_properties_to_be_enums()
        {
            var enumInfo = GetFirstProperty("Enum");

            enumInfo.Type.IsEnum.ShouldBeTrue("IsEnum");
            enumInfo.Type.IsEnumerable.ShouldBeFalse("IsEnumerable");
            enumInfo.Type.IsGeneric.ShouldBeFalse("IsGeneric");
            enumInfo.Type.IsNullable.ShouldBeFalse("IsNullable");
            enumInfo.Type.IsTask.ShouldBeFalse("IsTask");
            enumInfo.Type.IsPrimitive.ShouldBeTrue("IsPrimitive");
            enumInfo.Type.IsDate.ShouldBeFalse("IsDate");
            enumInfo.Type.IsGuid.ShouldBeFalse("IsGuid");
            enumInfo.Type.IsTimeSpan.ShouldBeFalse("IsTimeSpan");
        }

        [Fact]
        public void Expect_nullable_enum_properties_to_be_enums()
        {
            var nullableEnumInfo1 = GetFirstProperty("NullableEnum1");
            var nullableEnumInfo2 = GetFirstProperty("NullableEnum2");

            nullableEnumInfo1.Type.Name.ShouldEqual("ConsoleColor");
            nullableEnumInfo1.Type.OriginalName.ShouldEqual("ConsoleColor?");
            nullableEnumInfo1.Type.FullName.ShouldEqual("System.ConsoleColor?");

            nullableEnumInfo1.Type.IsEnum.ShouldBeTrue("IsEnum");
            nullableEnumInfo1.Type.IsEnumerable.ShouldBeFalse("IsEnumerable");
            nullableEnumInfo1.Type.IsGeneric.ShouldBeFalse("IsGeneric");
            nullableEnumInfo1.Type.IsNullable.ShouldBeTrue("IsNullable");
            nullableEnumInfo1.Type.IsTask.ShouldBeFalse("IsTask");
            nullableEnumInfo1.Type.IsPrimitive.ShouldBeTrue("IsPrimitive");
            nullableEnumInfo1.Type.IsDate.ShouldBeFalse("IsDate");
            nullableEnumInfo1.Type.IsGuid.ShouldBeFalse("IsGuid");
            nullableEnumInfo1.Type.IsTimeSpan.ShouldBeFalse("IsTimeSpan");

            nullableEnumInfo2.Type.Name.ShouldEqual("ConsoleColor");
            nullableEnumInfo2.Type.OriginalName.ShouldEqual("ConsoleColor?");
            nullableEnumInfo2.Type.FullName.ShouldEqual("System.ConsoleColor?");

            nullableEnumInfo2.Type.IsEnum.ShouldBeTrue("IsEnum");
            nullableEnumInfo2.Type.IsEnumerable.ShouldBeFalse("IsEnumerable");
            nullableEnumInfo2.Type.IsGeneric.ShouldBeFalse("IsGeneric");
            nullableEnumInfo2.Type.IsNullable.ShouldBeTrue("IsNullable");
            nullableEnumInfo2.Type.IsTask.ShouldBeFalse("IsTask");
            nullableEnumInfo2.Type.IsPrimitive.ShouldBeTrue("IsPrimitive");
            nullableEnumInfo2.Type.IsDate.ShouldBeFalse("IsDate");
            nullableEnumInfo2.Type.IsGuid.ShouldBeFalse("IsGuid");
            nullableEnumInfo2.Type.IsTimeSpan.ShouldBeFalse("IsTimeSpan");
        }

        [Fact]
        public void Expect_keyword_properties_to_return_keyword_name()
        {
            var intInfo = GetFirstProperty("Int");
            var nullableIntInfo1 = GetFirstProperty("NullableInt1");
            var nullableIntInfo2 = GetFirstProperty("NullableInt2");
            var enumerableIntInfo = GetFirstProperty("EnumerableInt");
            var enumerableNullableIntInfo = GetFirstProperty("EnumerableNullableInt");

            intInfo.Type.Name.ShouldEqual("number");
            intInfo.Type.OriginalName.ShouldEqual("int");
            intInfo.Type.FullName.ShouldEqual("System.Int32");

            nullableIntInfo1.Type.Name.ShouldEqual("number");
            nullableIntInfo1.Type.OriginalName.ShouldEqual("int?");
            nullableIntInfo1.Type.FullName.ShouldEqual("System.Int32?");

            nullableIntInfo2.Type.Name.ShouldEqual("number");
            nullableIntInfo2.Type.OriginalName.ShouldEqual("int?");
            nullableIntInfo2.Type.FullName.ShouldEqual("System.Int32?");

            enumerableIntInfo.Type.Name.ShouldEqual("number[]");
            enumerableIntInfo.Type.OriginalName.ShouldEqual("IEnumerable");
            enumerableIntInfo.Type.FullName.ShouldEqual("System.Collections.Generic.IEnumerable<System.Int32>");

            enumerableNullableIntInfo.Type.Name.ShouldEqual("number[]");
            enumerableNullableIntInfo.Type.OriginalName.ShouldEqual("IEnumerable");
            enumerableNullableIntInfo.Type.FullName.ShouldEqual("System.Collections.Generic.IEnumerable<System.Int32?>");
        }

        [Fact]
        public void Expect_complex_properties_not_to_be_primitive()
        {
            var propertyInfo = GetFirstProperty("Exception");

            propertyInfo.Type.IsPrimitive.ShouldBeFalse();
        }

        [Fact]
        public void Expect_enumerable_properties_to_be_enumerable()
        {
            var arrayInfo = GetFirstProperty("Array");
            var enumerableInfo = GetFirstProperty("Enumerable");
            var stringArrayInfo = GetFirstProperty("StringArray");
            var enumerableStringInfo = GetFirstProperty("EnumerableString");

            arrayInfo.Type.IsEnumerable.ShouldBeTrue();
            enumerableInfo.Type.IsEnumerable.ShouldBeTrue();
            stringArrayInfo.Type.IsEnumerable.ShouldBeTrue();
            enumerableStringInfo.Type.IsEnumerable.ShouldBeTrue();
        }

        [Fact]
        public void Expect_enumerable_properties_with_innerType_to_be_primitive()
        {
            var stringArrayInfo = GetFirstProperty("StringArray");
            var enumerableStringInfo = GetFirstProperty("EnumerableString");

            stringArrayInfo.Type.IsPrimitive.ShouldBeTrue();
            enumerableStringInfo.Type.IsPrimitive.ShouldBeTrue();
        }

        [Fact]
        public void Expect_enumerable_properties_without_innerType_not_to_be_primitive()
        {
            var arrayInfo = GetFirstProperty("Array");
            var enumerableInfo = GetFirstProperty("Enumerable");

            arrayInfo.Type.IsPrimitive.ShouldBeFalse();
            arrayInfo.Type.FullName.ShouldEqual("System.Array");
            enumerableInfo.Type.IsPrimitive.ShouldBeFalse();
        }

        [Fact]
        public void Expect_untyped_enumerable_properties_to_return_any_array()
        {
            var arrayInfo = GetFirstProperty("Array");
            var enumerableInfo = GetFirstProperty("Enumerable");

            arrayInfo.Type.Name.ShouldEqual("any[]");
            enumerableInfo.Type.Name.ShouldEqual("any[]");
        }

        [Fact]
        public void Expect_enumerable_properties_fullName()
        {
            var arrayInfo = GetFirstProperty("Array");
            var enumerableInfo = GetFirstProperty("Enumerable");
            var stringArrayInfo = GetFirstProperty("StringArray");
            var enumerableStringInfo = GetFirstProperty("EnumerableString");

            arrayInfo.Type.FullName.ShouldEqual("System.Array");
            enumerableInfo.Type.FullName.ShouldEqual("System.Collections.IEnumerable");
            stringArrayInfo.Type.FullName.ShouldEqual("System.Collections.Generic.ICollection<System.String>");
            enumerableStringInfo.Type.FullName.ShouldEqual("System.Collections.Generic.IEnumerable<System.String>");
        }

        [Fact]
        public void Expect_typed_enumerable_properties_to_return_typed_array()
        {
            var stringArrayInfo = GetFirstProperty("StringArray");
            var enumerableStringInfo = GetFirstProperty("EnumerableString");
            var listStringInfo = GetFirstProperty("ListString");

            enumerableStringInfo.Type.ToString().ShouldEqual("string[]");
            listStringInfo.Type.Name.ShouldEqual("string[]");

            stringArrayInfo.Type.ToString().ShouldEqual("string[]");

        }

        [Fact]
        public void Expect_string_properties_not_to_be_enumerable()
        {
            var stringInfo = GetFirstProperty("String");

            stringInfo.Type.IsEnumerable.ShouldBeFalse();
        }

        [Fact]
        public void Expect_generic_property_type_to_match_generic_argument()
        {
            var genericInfo = fileInfo.Classes
                .First(c => c.Name == "GenericPropertyInfo").Properties
                .First(p => p.Name == "Generic");

            genericInfo.Type.Name.ShouldEqual("T");
            genericInfo.Type.FullName.ShouldEqual("T");
        }

        [Fact]
        public void Expect_enumerable_generic_property_inner_type_to_match_generic_argument()
        {
            var genericInfo = fileInfo.Classes
                .First(c => c.Name == "GenericPropertyInfo").Properties
                .First(p => p.Name == "EnumerableGeneric");
            var innerType = genericInfo.Type.TypeArguments.First();

            genericInfo.Type.IsEnumerable.ShouldBeTrue();
            genericInfo.Type.IsGeneric.ShouldBeTrue();

            genericInfo.Type.Name.ShouldEqual("T[]");
            genericInfo.Type.OriginalName.ShouldEqual("IEnumerable");
            genericInfo.Type.FullName.ShouldEqual("System.Collections.Generic.IEnumerable<T>");

            genericInfo.Type.TypeArguments.Count.ShouldEqual(1);
            innerType.Name.ShouldEqual("T");
            innerType.FullName.ShouldEqual("T");
        }
    }
}