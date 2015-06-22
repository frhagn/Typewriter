using System.Linq;
using Should;
using Typewriter.CodeModel;
using Typewriter.Tests.TestInfrastructure;
using Xunit;

namespace Typewriter.Tests.CodeModel
{
    [Trait("CodeModel", "Interfaces")]
    public class InterfaceTests : TestBase
    {
        private readonly File fileInfo = GetFile(@"Tests\CodeModel\Support\IInterfaceInfo.cs");

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
            interfaceInfo.GenericTypeArguments.Count.ShouldEqual(0);
        }

        [Fact]
        public void Expect_generic_interface_to_be_generic()
        {
            var interfaceInfo = fileInfo.Interfaces.First(i => i.Name == "IGenericInterface");
            var genericTypeArgument = interfaceInfo.GenericTypeArguments.First();

            interfaceInfo.IsGeneric.ShouldBeTrue();
            interfaceInfo.GenericTypeArguments.Count.ShouldEqual(1);
            genericTypeArgument.Name.ShouldEqual("T");
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
    }
}