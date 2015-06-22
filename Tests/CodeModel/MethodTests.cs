using System.Linq;
using Should;
using Typewriter.CodeModel;
using Typewriter.Tests.TestInfrastructure;
using Xunit;

namespace Typewriter.Tests.CodeModel
{
    [Trait("CodeModel", "Methods")]
    public class MethodTests : TestBase
    {
        private readonly File fileInfo = GetFile(@"Tests\CodeModel\Support\MethodInfo.cs");

        [Fact]
        public void Expect_name_to_match_method_name()
        {
            var classInfo = fileInfo.Classes.First();
            var methodInfo = classInfo.Methods.First(p => p.Name == "Method");

            methodInfo.Name.ShouldEqual("Method");
            methodInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.MethodInfo.Method");
            methodInfo.Parent.ShouldEqual(classInfo);
        }

        [Fact]
        public void Expect_to_find_attributes()
        {
            var classInfo = fileInfo.Classes.First();
            var methodInfo = classInfo.Methods.First(p => p.Name == "Method");
            var attributeInfo = methodInfo.Attributes.First();

            methodInfo.Attributes.Count.ShouldEqual(1);
            attributeInfo.Name.ShouldEqual("AttributeInfo");
            attributeInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.AttributeInfoAttribute");
        }

        [Fact]
        public void Expect_to_find_parameters()
        {
            var classInfo = fileInfo.Classes.First();
            var methodInfo = classInfo.Methods.First(p => p.Name == "Method");
            var parameterInfo = methodInfo.Parameters.First();

            methodInfo.Parameters.Count.ShouldEqual(1);
            parameterInfo.Name.ShouldEqual("parameter");
        }

        [Fact]
        public void Expect_to_find_parameter_attributes()
        {
            var classInfo = fileInfo.Classes.First();
            var methodInfo = classInfo.Methods.First(p => p.Name == "Method");
            var parameterInfo = methodInfo.Parameters.First();
            var attributeInfo = parameterInfo.Attributes.First();

            parameterInfo.Attributes.Count.ShouldEqual(1);
            attributeInfo.Name.ShouldEqual("AttributeInfo");
            attributeInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.AttributeInfoAttribute");
        }

        [Fact]
        public void Expect_void_methods_to_return_void()
        {
            var classInfo = fileInfo.Classes.First();
            var methodInfo = classInfo.Methods.First(p => p.Name == "Method");
            
            methodInfo.Type.Name.ShouldEqual("Void");
            methodInfo.Type.IsEnum.ShouldBeFalse("IsEnum");
            methodInfo.Type.IsEnumerable.ShouldBeFalse("IsEnumerable");
            methodInfo.Type.IsGeneric.ShouldBeFalse("IsGeneric");
            methodInfo.Type.IsNullable.ShouldBeFalse("IsNullable");
            methodInfo.Type.IsPrimitive.ShouldBeFalse("IsPrimitive");
        }

        [Fact]
        public void Expect_generic_methods_to_handle_generic_type_arguments()
        {
            var classInfo = fileInfo.Classes.First();
            var methodInfo = classInfo.Methods.First(p => p.Name == "Generic");
            var genericTypeInfo = methodInfo.GenericTypeArguments.First();
            var parameterTypeInfo = methodInfo.Parameters.First().Type;

            methodInfo.IsGeneric.ShouldBeTrue("IsGeneric");
            methodInfo.GenericTypeArguments.Count.ShouldEqual(1);

            methodInfo.Type.Name.ShouldEqual("T");
            methodInfo.Type.FullName.ShouldEqual("T");
            genericTypeInfo.Name.ShouldEqual("T");
            genericTypeInfo.FullName.ShouldEqual("T");
            parameterTypeInfo.Name.ShouldEqual("T");
            parameterTypeInfo.FullName.ShouldEqual("T");
        }

        [Fact]
        public void Expect_methods_to_handle_generic_type_arguments_from_class()
        {
            var classInfo = fileInfo.Classes.First(c => c.Name == "GenericMethodInfo");
            var methodInfo = classInfo.Methods.First(p => p.Name == "Method");
            var parameterTypeInfo = methodInfo.Parameters.First().Type;

            methodInfo.IsGeneric.ShouldBeFalse("IsGeneric");
            methodInfo.GenericTypeArguments.Count.ShouldEqual(0);

            methodInfo.Type.Name.ShouldEqual("T");
            methodInfo.Type.FullName.ShouldEqual("T");
            parameterTypeInfo.Name.ShouldEqual("T");
            parameterTypeInfo.FullName.ShouldEqual("T");
        }

        [Fact]
        public void Expect_generic_methods_to_handle_generic_type_arguments_from_class_and_method()
        {
            var classInfo = fileInfo.Classes.First(c => c.Name == "GenericMethodInfo");
            var methodInfo = classInfo.Methods.First(p => p.Name == "Generic");
            var firstParameterTypeInfo = methodInfo.Parameters.First(p => p.Name == "parameter1").Type;
            var secondParameterTypeInfo = methodInfo.Parameters.First(p => p.Name == "parameter2").Type;

            methodInfo.IsGeneric.ShouldBeTrue("IsGeneric");
            methodInfo.GenericTypeArguments.Count.ShouldEqual(1);

            methodInfo.Type.Name.ShouldEqual("T1");
            methodInfo.Type.FullName.ShouldEqual("T1");
            firstParameterTypeInfo.Name.ShouldEqual("T1");
            firstParameterTypeInfo.FullName.ShouldEqual("T1");
            secondParameterTypeInfo.Name.ShouldEqual("T");
            secondParameterTypeInfo.FullName.ShouldEqual("T");
        }
    }
}