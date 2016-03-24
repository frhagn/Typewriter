using System.Linq;
using Should;
using Typewriter.CodeModel;
using Typewriter.Extensions.Types;
using Typewriter.Tests.TestInfrastructure;
using Xunit;

namespace Typewriter.Tests.CodeModel
{
    [Trait("CodeModel", "Types"), Collection(nameof(CodeDomFixture))]
    public class CodeDomTypeTests : TypeTests
    {
        public CodeDomTypeTests(CodeDomFixture fixture) : base(fixture)
        {
        }
    }

    [Trait("CodeModel", "Types"), Collection(nameof(RoslynFixture))]
    public class RoslynTypeTests : TypeTests
    {
        public RoslynTypeTests(RoslynFixture fixture) : base(fixture)
        {
        }
    }

    public abstract class TypeTests : TestBase
    {
        private readonly File fileInfo;

        protected TypeTests(ITestFixture fixture) : base(fixture)
        {
            fileInfo = GetFile(@"Tests\CodeModel\Support\TypeInfo.cs");
        }

        [Fact]
        public void Expect_type_name_to_match_class_name()
        {
            var classInfo = fileInfo.Classes.First();
            var propertyInfo = classInfo.Properties.First(p => p.Name == "Class");
            var typeInfo = propertyInfo.Type;

            typeInfo.Name.ShouldEqual("ClassInfo");
            typeInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.ClassInfo");
            typeInfo.Namespace.ShouldEqual("Typewriter.Tests.CodeModel.Support");
            typeInfo.Parent.ShouldEqual(propertyInfo);
        }

        [Fact]
        public void Expect_to_find_doc_comment()
        {
            var classInfo = fileInfo.Classes.First();
            var propertyInfo = classInfo.Properties.First(p => p.Name == "Class");
            var typeInfo = propertyInfo.Type;
            typeInfo.DocComment.Summary.ShouldEqual("summary");
        }

        [Fact]
        public void Expect_to_find_type_attributes()
        {
            var classInfo = fileInfo.Classes.First();
            var typeInfo = classInfo.Properties.First(p => p.Name == "Class").Type;
            var attributeInfo = typeInfo.Attributes.First();

            typeInfo.Attributes.Count.ShouldEqual(1);
            attributeInfo.Name.ShouldEqual("AttributeInfo");
            attributeInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.AttributeInfoAttribute");
        }

        [Fact]
        public void Expect_to_find_type_base_class()
        {
            var classInfo = fileInfo.Classes.First();
            var typeInfo = classInfo.Properties.First(p => p.Name == "Class").Type;
            var baseClassInfo = typeInfo.BaseClass;
            var propertyInfo = baseClassInfo.Properties.First();

            baseClassInfo.Name.ShouldEqual("BaseClassInfo");

            baseClassInfo.Properties.Count.ShouldEqual(1);
            propertyInfo.Name.ShouldEqual("PublicBaseProperty");
        }

        [Fact]
        public void Expect_not_to_find_object_base_class()
        {
            var classInfo = fileInfo.Classes.First();
            var typeInfo = classInfo.Properties.First(p => p.Name == "BaseClass").Type;

            typeInfo.BaseClass.ShouldBeNull();
        }

        [Fact]
        public void Expect_to_find_type_interfaces()
        {
            if (isRoslyn)
            {
                var classInfo = fileInfo.Classes.First();
                var typeInfo = classInfo.Properties.First(p => p.Name == "Class").Type;
                var interfaceInfo = typeInfo.Interfaces.First();
                var propertyInfo = interfaceInfo.Properties.First();

                typeInfo.Interfaces.Count.ShouldEqual(1);
                interfaceInfo.Name.ShouldEqual("IInterfaceInfo");

                interfaceInfo.Properties.Count.ShouldEqual(1);
                propertyInfo.Name.ShouldEqual("PublicProperty");
            }
        }

        [Fact]
        public void Expect_non_generic_type_not_to_be_generic()
        {
            var classInfo = fileInfo.Classes.First();
            var propertyInfo = classInfo.Properties.First(p => p.Name == "Class");

            propertyInfo.Type.IsGeneric.ShouldBeFalse();
            propertyInfo.Type.TypeArguments.Count.ShouldEqual(0);
        }

        [Fact]
        public void Expect_generic_type_to_be_generic()
        {
            var classInfo = fileInfo.Classes.First();
            var typeInfo = classInfo.Properties.First(p => p.Name == "GenericClass").Type;
            var genericTypeArgument = typeInfo.TypeArguments.First();

            typeInfo.IsGeneric.ShouldBeTrue();
            typeInfo.TypeArguments.Count.ShouldEqual(1);
            genericTypeArgument.Name.ShouldEqual("string");

            if (isRoslyn)
            {
                var genericTypeParameter = typeInfo.TypeParameters.First();
                typeInfo.TypeParameters.Count.ShouldEqual(1);
                genericTypeParameter.Name.ShouldEqual("T");
            }
        }
        
        [Fact]
        public void Expect_to_find_public_constants()
        {
            var classInfo = fileInfo.Classes.First();
            var typeInfo = classInfo.Properties.First(p => p.Name == "Class").Type;
            var constantInfo = typeInfo.Constants.First();

            typeInfo.Constants.Count.ShouldEqual(1);
            constantInfo.Name.ShouldEqual("PublicConstant");
        }

        [Fact]
        public void Expect_to_find_public_delegates()
        {
            var classInfo = fileInfo.Classes.First();
            var typeInfo = classInfo.Properties.First(p => p.Name == "Class").Type;
            var delegateInfo = typeInfo.Delegates.First();

            typeInfo.Delegates.Count.ShouldEqual(1);
            delegateInfo.Name.ShouldEqual("PublicDelegate");
        }

        [Fact]
        public void Expect_to_find_public_fields()
        {
            var classInfo = fileInfo.Classes.First();
            var typeInfo = classInfo.Properties.First(p => p.Name == "Class").Type;
            var fieldInfo = typeInfo.Fields.First();

            typeInfo.Fields.Count.ShouldEqual(1);
            fieldInfo.Name.ShouldEqual("PublicField");
        }

        [Fact]
        public void Expect_to_find_public_methods()
        {
            var classInfo = fileInfo.Classes.First();
            var typeInfo = classInfo.Properties.First(p => p.Name == "Class").Type;
            var methodInfo = typeInfo.Methods.First();

            typeInfo.Methods.Count.ShouldEqual(1);
            methodInfo.Name.ShouldEqual("PublicMethod");
        }

        [Fact]
        public void Expect_to_find_public_properties()
        {
            var classInfo = fileInfo.Classes.First();
            var typeInfo = classInfo.Properties.First(p => p.Name == "Class").Type;
            var propertyInfo = typeInfo.Properties.First();

            typeInfo.Properties.Count.ShouldEqual(1);
            propertyInfo.Name.ShouldEqual("PublicProperty");
        }

        [Fact]
        public void Expect_to_find_nested_public_classes()
        {
            var classInfo = fileInfo.Classes.First();
            var typeInfo = classInfo.Properties.First(p => p.Name == "Class").Type;
            var nestedClassInfo = typeInfo.NestedClasses.First();
            var propertyInfo = nestedClassInfo.Properties.First();

            typeInfo.NestedClasses.Count.ShouldEqual(1);
            nestedClassInfo.Name.ShouldEqual("NestedClassInfo");

            nestedClassInfo.Properties.Count.ShouldEqual(1);
            propertyInfo.Name.ShouldEqual("PublicNestedProperty");
        }

        [Fact]
        public void Expect_to_find_nested_public_enums()
        {
            var classInfo = fileInfo.Classes.First();
            var typeInfo = classInfo.Properties.First(p => p.Name == "Class").Type;
            var nestedEnumInfo = typeInfo.NestedEnums.First();
            var valueInfo = nestedEnumInfo.Values.First();

            typeInfo.NestedEnums.Count.ShouldEqual(1);
            nestedEnumInfo.Name.ShouldEqual("NestedEnumInfo");

            nestedEnumInfo.Values.Count.ShouldEqual(1);
            valueInfo.Name.ShouldEqual("NestedValue");
        }

        [Fact]
        public void Expect_to_find_nested_public_interfaces()
        {
            var classInfo = fileInfo.Classes.First();
            var typeInfo = classInfo.Properties.First(p => p.Name == "Class").Type;
            var nestedInterfaceInfo = typeInfo.NestedInterfaces.First();
            var propertyInfo = nestedInterfaceInfo.Properties.First();

            typeInfo.NestedInterfaces.Count.ShouldEqual(1);
            nestedInterfaceInfo.Name.ShouldEqual("INestedInterfaceInfo");

            nestedInterfaceInfo.Properties.Count.ShouldEqual(1);
            propertyInfo.Name.ShouldEqual("PublicNestedProperty");
        }

        [Fact]
        public void Expect_to_find_containing_class_on_nested_class()
        {
            var classInfo = fileInfo.Classes.First();
            var typeInfo = classInfo.Properties.First(p => p.Name == "Class").Type;
            var nestedClassInfo = typeInfo.NestedClasses.First();
            var containingClassInfo = nestedClassInfo.ContainingClass;

            containingClassInfo.Name.ShouldEqual("ClassInfo");
        }

        [Fact]
        public void Expect_not_to_find_containing_class_on_top_level_class()
        {
            var classInfo = fileInfo.Classes.First();
            var typeInfo = classInfo.Properties.First(p => p.Name == "Class").Type;
            var containingClassInfo = typeInfo.ContainingClass;

            containingClassInfo.ShouldBeNull();
        }

        [Fact]
        public void Expect_generic_baseclass_to_have_type_arguments()
        {
            var classInfo = fileInfo.Classes.First();
            var typeInfo = classInfo.Properties.First(p => p.Name == "InheritGenericClass").Type;
            var genericTypeArgument = typeInfo.BaseClass.TypeArguments.First();

            typeInfo.BaseClass.IsGeneric.ShouldBeTrue();
            typeInfo.BaseClass.TypeArguments.Count.ShouldEqual(1);

            genericTypeArgument.Name.ShouldEqual("string");

            if (isRoslyn)
            {
                var genericTypeParameter = typeInfo.BaseClass.TypeParameters.First();
                typeInfo.BaseClass.TypeParameters.Count.ShouldEqual(1);
                genericTypeParameter.Name.ShouldEqual("T");
            }
        }

        [Fact]
        public void Expect_types_defined_in_solution_to_return_isdefined()
        {
            var classInfo = fileInfo.Classes.First();
            var typeInfo = classInfo.Properties.First(p => p.Name == "Class").Type;
            var genericTypeInfo = classInfo.Properties.First(p => p.Name == "GenericClass").Type;
            var classCollection = classInfo.Properties.First(p => p.Name == "ClassCollection").Type;

            typeInfo.IsDefined.ShouldBeTrue("Class.IsDefined");
            if (isRoslyn) genericTypeInfo.IsDefined.ShouldBeTrue("GenericClass.IsDefined");
            classCollection.Unwrap().IsDefined.ShouldBeTrue("ClassCollection.IsDefined");
        }

        [Fact]
        public void Expect_external_types_not_to_return_isdefined()
        {
            var classInfo = fileInfo.Classes.First();
            var typeInfo = classInfo.Properties.First(p => p.Name == "String").Type;
            var genericTypeInfo = classInfo.Properties.First(p => p.Name == "GenericClass").Type;
            var classCollection = classInfo.Properties.First(p => p.Name == "ClassCollection").Type;

            typeInfo.IsDefined.ShouldBeFalse("String.IsDefined");
            genericTypeInfo.Unwrap().IsDefined.ShouldBeFalse("GenericClass.IsDefined");
            classCollection.IsDefined.ShouldBeFalse("ClassCollection.IsDefined");
        }
    }
}
