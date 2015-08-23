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

        protected PropertyTests(ITestFixture fixture) : base(fixture)
        {
            fileInfo = GetFile(@"Tests\CodeModel\Support\PropertyInfo.cs");
        }

        [Fact]
        public void Expect_name_to_match_property_name()
        {
            var classInfo = fileInfo.Classes.First();
            var propertyInfo = classInfo.Properties.First(p => p.Name == "Bool");
            
            propertyInfo.Name.ShouldEqual("Bool");
            propertyInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.PropertyInfo.Bool");
            propertyInfo.Parent.ShouldEqual(classInfo);
        }

        [Fact]
        public void Expect_to_find_attributes()
        {
            var classInfo = fileInfo.Classes.First();
            var propertyInfo = classInfo.Properties.First(p => p.Name == "Bool");
            var attributeInfo = propertyInfo.Attributes.First();

            propertyInfo.Attributes.Count.ShouldEqual(1);
            attributeInfo.Name.ShouldEqual("AttributeInfo");
            attributeInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.AttributeInfoAttribute");
        }

        [Fact]
        public void Expect_properties_without_public_getter_to_only_have_setter()
        {
            var classInfo = fileInfo.Classes.First();
            var setterOnly = classInfo.Properties.First(p => p.Name == "SetterOnly");
            var privateGetter = classInfo.Properties.First(p => p.Name == "PrivateGetter");

            setterOnly.HasGetter.ShouldBeFalse();
            setterOnly.HasSetter.ShouldBeTrue();
            privateGetter.HasGetter.ShouldBeFalse();
            privateGetter.HasSetter.ShouldBeTrue();
        }

        [Fact]
        public void Expect_properties_without_public_setter_to_only_have_getter()
        {
            var classInfo = fileInfo.Classes.First();
            var getterOnly = classInfo.Properties.First(p => p.Name == "GetterOnly");
            var privateSetter = classInfo.Properties.First(p => p.Name == "PrivateSetter");

            getterOnly.HasGetter.ShouldBeTrue();
            getterOnly.HasSetter.ShouldBeFalse();
            privateSetter.HasGetter.ShouldBeTrue();
            privateSetter.HasSetter.ShouldBeFalse();
        }

        [Fact]
        public void Expect_properties_with_public_getter_and_setter_to_have_both_getter_and_setter()
        {
            var classInfo = fileInfo.Classes.First();
            var propertyInfo = classInfo.Properties.First(p => p.Name == "Bool");

            propertyInfo.HasGetter.ShouldBeTrue();
            propertyInfo.HasSetter.ShouldBeTrue();
        }

        [Fact]
        public void Expect_primitive_properties_to_be_primitive()
        {
            var classInfo = fileInfo.Classes.First();
            var properties = new[] { "Bool", "Char", "String", "Byte", "Sbyte", "Int", "Uint", "Short", "Ushort", "Long", "Ulong", "Float", "Double", "Decimal" };

            foreach (var property in properties)
            {
                var propertyInfo = classInfo.Properties.First(p => p.Name == property);

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
            var classInfo = fileInfo.Classes.First();
            var properties = new[] { "DateTime", "DateTimeOffset" };

            foreach (var property in properties)
            {
                var propertyInfo = classInfo.Properties.First(p => p.Name == property);

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
            var classInfo = fileInfo.Classes.First();
            var propertyInfo = classInfo.Properties.First(p => p.Name == "Guid");

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
            var classInfo = fileInfo.Classes.First();
            var propertyInfo = classInfo.Properties.First(p => p.Name == "TimeSpan");

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
            var classInfo = fileInfo.Classes.First();
            var objectInfo = classInfo.Properties.First(p => p.Name == "Object");
            var dynamicInfo = classInfo.Properties.First(p => p.Name == "Dynamic");

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
            var classInfo = fileInfo.Classes.First();
            var enumInfo = classInfo.Properties.First(p => p.Name == "Enum");

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
            var classInfo = fileInfo.Classes.First();

            var nullableEnumInfo1 = classInfo.Properties.First(p => p.Name == "NullableEnum1");
            var nullableEnumInfo2 = classInfo.Properties.First(p => p.Name == "NullableEnum2");

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
            var classInfo = fileInfo.Classes.First();
            var intInfo = classInfo.Properties.First(p => p.Name == "Int");
            var nullableIntInfo1 = classInfo.Properties.First(p => p.Name == "NullableInt1");
            var nullableIntInfo2 = classInfo.Properties.First(p => p.Name == "NullableInt2");
            var enumerableIntInfo = classInfo.Properties.First(p => p.Name == "EnumerableInt");
            var enumerableNullableIntInfo = classInfo.Properties.First(p => p.Name == "EnumerableNullableInt");

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
            var classInfo = fileInfo.Classes.First();
            var propertyInfo = classInfo.Properties.First(p => p.Name == "Exception");

            propertyInfo.Type.IsPrimitive.ShouldBeFalse();
        }

        [Fact]
        public void Expect_enumerable_properties_to_be_enumerable()
        {
            var classInfo = fileInfo.Classes.First();
            var arrayInfo = classInfo.Properties.First(p => p.Name == "Array");
            var enumerableInfo = classInfo.Properties.First(p => p.Name == "Enumerable");
            var stringArrayInfo = classInfo.Properties.First(p => p.Name == "StringArray");
            var enumerableStringInfo = classInfo.Properties.First(p => p.Name == "EnumerableString");

            arrayInfo.Type.IsEnumerable.ShouldBeTrue();
            enumerableInfo.Type.IsEnumerable.ShouldBeTrue();
            stringArrayInfo.Type.IsEnumerable.ShouldBeTrue();
            enumerableStringInfo.Type.IsEnumerable.ShouldBeTrue();
        }

        [Fact]
        public void Expect_untyped_enumerable_properties_to_return_any_array()
        {
            var classInfo = fileInfo.Classes.First();
            var arrayInfo = classInfo.Properties.First(p => p.Name == "Array");
            var enumerableInfo = classInfo.Properties.First(p => p.Name == "Enumerable");

            arrayInfo.Type.Name.ShouldEqual("any[]");
            enumerableInfo.Type.Name.ShouldEqual("any[]");
        }

        [Fact]
        public void Expect_typed_enumerable_properties_to_return_typed_array()
        {
            var classInfo = fileInfo.Classes.First();
            var stringArrayInfo = classInfo.Properties.First(p => p.Name == "StringArray");
            var enumerableStringInfo = classInfo.Properties.First(p => p.Name == "EnumerableString");
            var listStringInfo = classInfo.Properties.First(p => p.Name == "ListString");

            enumerableStringInfo.Type.ToString().ShouldEqual("string[]");
            listStringInfo.Type.Name.ShouldEqual("string[]");
            
            stringArrayInfo.Type.ToString().ShouldEqual("string[]");

        }

        [Fact]
        public void Expect_string_properties_not_to_be_enumerable()
        {
            var classInfo = fileInfo.Classes.First();
            var stringInfo = classInfo.Properties.First(p => p.Name == "String");

            stringInfo.Type.IsEnumerable.ShouldBeFalse();
        }

        [Fact]
        public void Expect_generic_property_type_to_match_generic_argument()
        {
            var classInfo = fileInfo.Classes.First(c => c.Name == "GenericPropertyInfo");
            var genericInfo = classInfo.Properties.First(p => p.Name == "Generic");

            genericInfo.Type.Name.ShouldEqual("T");
            genericInfo.Type.FullName.ShouldEqual("T");
        }

        [Fact]
        public void Expect_enumerable_generic_property_inner_type_to_match_generic_argument()
        {
            var classInfo = fileInfo.Classes.First(c => c.Name == "GenericPropertyInfo");
            var genericInfo = classInfo.Properties.First(p => p.Name == "EnumerableGeneric");
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