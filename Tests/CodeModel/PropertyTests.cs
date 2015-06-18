using System.Linq;
using Should;
using Tests.TestInfrastructure;
using Xunit;
using Typewriter.CodeModel;

namespace Tests.CodeModel
{
    [Trait("CodeModel", "Properties")]
    public class PropertyTests : TestBase
    {
        private readonly File fileInfo = GetFile(@"Tests\CodeModel\Support\PropertyInfo.cs");

        [Fact]
        public void Expect_name_to_match_property_name()
        {
            var classInfo = fileInfo.Classes.First();
            var propertyInfo = classInfo.Properties.First(p => p.Name == "Bool");

            propertyInfo.Name.ShouldEqual("Bool");
            propertyInfo.FullName.ShouldEqual("Tests.CodeModel.Support.PropertyInfo.Bool");
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

                propertyInfo.IsEnum.ShouldBeFalse($"IsEnum {property}");
                propertyInfo.IsEnumerable.ShouldBeFalse($"IsEnumerable {property}");
                propertyInfo.IsGeneric.ShouldBeFalse($"IsGeneric {property}");
                propertyInfo.IsNullable.ShouldBeFalse($"IsNullable {property}");
                propertyInfo.IsPrimitive.ShouldBeTrue($"IsPrimitive {property}");
            }
        }

        [Fact]
        public void Expect_complex_properties_not_to_be_primitive()
        {
            var classInfo = fileInfo.Classes.First();
            var propertyInfo = classInfo.Properties.First(p => p.Name == "Exception");

            propertyInfo.IsPrimitive.ShouldBeFalse();
        }

        [Fact]
        public void Expect_enumerable_properties_to_be_enumerable()
        {
            var classInfo = fileInfo.Classes.First();
            var arrayInfo = classInfo.Properties.First(p => p.Name == "Array");
            var enumerableInfo = classInfo.Properties.First(p => p.Name == "Enumerable");
            var stringArrayInfo = classInfo.Properties.First(p => p.Name == "StringArray");
            var enumerableStringInfo = classInfo.Properties.First(p => p.Name == "EnumerableString");

            arrayInfo.IsEnumerable.ShouldBeTrue();
            enumerableInfo.IsEnumerable.ShouldBeTrue();
            stringArrayInfo.IsEnumerable.ShouldBeTrue();
            enumerableStringInfo.IsEnumerable.ShouldBeTrue();
        }

        [Fact]
        public void Expect_string_properties_not_to_be_enumerable()
        {
            var classInfo = fileInfo.Classes.First();
            var stringInfo = classInfo.Properties.First(p => p.Name == "String");

            stringInfo.IsEnumerable.ShouldBeFalse();
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
            var innerType = genericInfo.Type.GenericTypeArguments.First();

            genericInfo.IsEnumerable.ShouldBeTrue();
            genericInfo.IsGeneric.ShouldBeTrue();

            genericInfo.Type.Name.ShouldEqual("IEnumerable");
            genericInfo.Type.FullName.ShouldEqual("System.Collections.Generic.IEnumerable<T>");

            genericInfo.Type.GenericTypeArguments.Count.ShouldEqual(1);
            innerType.Name.ShouldEqual("T");
            innerType.FullName.ShouldEqual("T");
        }
    }
}