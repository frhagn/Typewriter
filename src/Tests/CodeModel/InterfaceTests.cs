using System.Linq;
using Should;
using Typewriter.CodeModel;
using Typewriter.Tests.TestInfrastructure;
using Xunit;

namespace Typewriter.Tests.CodeModel
{
    [Trait("CodeModel", "Interfaces"), Collection(nameof(CodeDomFixture))]
    public class CodeDomInterfaceTests : InterfaceTests
    {
        public CodeDomInterfaceTests(CodeDomFixture fixture) : base(fixture)
        {
        }
    }

    [Trait("CodeModel", "Interfaces"), Collection(nameof(RoslynFixture))]
    public class RoslynInterfaceTests : InterfaceTests
    {
        public RoslynInterfaceTests(RoslynFixture fixture) : base(fixture)
        {
        }
    }

    public abstract class InterfaceTests : TestBase
    {
        private readonly File fileInfo;

        protected InterfaceTests(ITestFixture fixture) : base(fixture)
        {
            fileInfo = GetFile(@"Tests\CodeModel\Support\IInterfaceInfo.cs");
        }

        [Fact]
        public void Expect_name_to_match_interface_name()
        {
            var interfaceInfo = fileInfo.Interfaces.First();

            interfaceInfo.Name.ShouldEqual("IInterfaceInfo");
            interfaceInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.IInterfaceInfo");
            interfaceInfo.Namespace.ShouldEqual("Typewriter.Tests.CodeModel.Support");
            interfaceInfo.Parent.ShouldEqual(fileInfo);
        }

        [Fact]
        public void Expect_to_find_doc_comment()
        {
            var interfaceInfo = fileInfo.Interfaces.First();
            interfaceInfo.DocComment.Summary.ShouldEqual("summary");
        }

        [Fact]
        public void Expect_to_find_attributes()
        {
            var interfaceInfo = fileInfo.Interfaces.First();
            var attributeInfo = interfaceInfo.Attributes.First();

            interfaceInfo.Attributes.Count.ShouldEqual(1);
            attributeInfo.Name.ShouldEqual("AttributeInfo");
            attributeInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.AttributeInfoAttribute");
        }

        [Fact]
        public void Expect_non_generic_interface_not_to_be_generic()
        {
            var interfaceInfo = fileInfo.Interfaces.First();

            interfaceInfo.IsGeneric.ShouldBeFalse();
            interfaceInfo.TypeParameters.Count.ShouldEqual(0);
        }

        [Fact]
        public void Expect_generic_interface_to_be_generic()
        {
            var interfaceInfo = fileInfo.Interfaces.First(i => i.Name == "IGenericInterface");
            var genericTypeArgument = interfaceInfo.TypeParameters.First();

            interfaceInfo.IsGeneric.ShouldBeTrue();
            interfaceInfo.TypeParameters.Count.ShouldEqual(1);
            genericTypeArgument.Name.ShouldEqual("T");
        }

        [Fact]
        public void Expect_to_find_public_events()
        {
            var interfaceInfo = fileInfo.Interfaces.First();
            var delegateInfo = interfaceInfo.Events.First();

            interfaceInfo.Events.Count.ShouldEqual(1);
            delegateInfo.Name.ShouldEqual("PublicEvent");
        }

        [Fact]
        public void Expect_to_find_interfaces()
        {
            var interfaceInfo = fileInfo.Interfaces.First();
            var implementedInterfaceInfo = interfaceInfo.Interfaces.First();
            var propertyInfo = implementedInterfaceInfo.Properties.First();

            interfaceInfo.Interfaces.Count.ShouldEqual(1);
            implementedInterfaceInfo.Name.ShouldEqual("IBaseInterfaceInfo");

            implementedInterfaceInfo.Properties.Count.ShouldEqual(1);
            propertyInfo.Name.ShouldEqual("PublicBaseProperty");
        }

        [Fact]
        public void Expect_to_find_methods()
        {
            var interfaceInfo = fileInfo.Interfaces.First();
            var methodInfo = interfaceInfo.Methods.First();

            interfaceInfo.Methods.Count.ShouldEqual(1);
            methodInfo.Name.ShouldEqual("PublicMethod");
        }

        [Fact]
        public void Expect_to_find_properties()
        {
            var interfaceInfo = fileInfo.Interfaces.First();
            var propertyInfo = interfaceInfo.Properties.First();

            interfaceInfo.Properties.Count.ShouldEqual(1);
            propertyInfo.Name.ShouldEqual("PublicProperty");
        }

        [Fact]
        public void Expect_to_find_containing_class_on_nested_interface()
        {
            var classInfo = fileInfo.Classes.First();
            var nestedInterfaceInfo = classInfo.NestedInterfaces.First();
            var containingClassInfo = nestedInterfaceInfo.ContainingClass;

            containingClassInfo.Name.ShouldEqual("InterfaceContiningClassInfo");
        }

        [Fact]
        public void Expect_not_to_find_containing_class_on_top_level_interface()
        {
            var interfaceInfo = fileInfo.Interfaces.First();
            var containingClassInfo = interfaceInfo.ContainingClass;

            containingClassInfo.ShouldBeNull();
        }

        [Fact]
        public void Expect_inherited_generic_interface_to_have_type_arguments()
        {
            var interfaceInfo = fileInfo.Interfaces.First(m => m.Name == "IInheritGenericInterfaceInfo");
            var genericTypeArgument = interfaceInfo.Interfaces.First().TypeArguments.First();

            interfaceInfo.Interfaces.First().IsGeneric.ShouldBeTrue();
            interfaceInfo.Interfaces.First().TypeArguments.Count.ShouldEqual(1);

            genericTypeArgument.Name.ShouldEqual("string");

            if (isRoslyn)
            {
                var genericTypeParameter = interfaceInfo.Interfaces.First().TypeParameters.First();
                interfaceInfo.Interfaces.First().TypeParameters.Count.ShouldEqual(1);
                genericTypeParameter.Name.ShouldEqual("T");
            }
        }
    }
}