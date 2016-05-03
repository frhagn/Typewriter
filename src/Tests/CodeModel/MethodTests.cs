using System.Linq;
using Should;
using Typewriter.CodeModel;
using Typewriter.Tests.TestInfrastructure;
using Xunit;

namespace Typewriter.Tests.CodeModel
{
    [Trait("CodeModel", "Methods"), Collection(nameof(CodeDomFixture))]
    public class CodeDomMethodTests : MethodTests
    {
        public CodeDomMethodTests(CodeDomFixture fixture) : base(fixture)
        {
        }
    }

    [Trait("CodeModel", "Methods"), Collection(nameof(RoslynFixture))]
    public class RoslynMethodTests : MethodTests
    {
        public RoslynMethodTests(RoslynFixture fixture) : base(fixture)
        {
        }
    }

    public abstract class MethodTests : TestBase
    {
        private readonly File fileInfo;

        protected MethodTests(ITestFixture fixture) : base(fixture)
        {
            fileInfo = GetFile(@"Tests\CodeModel\Support\MethodInfo.cs");
        }

        [Fact]
        public void Expect_name_to_match_method_name()
        {
            var classInfo = fileInfo.Classes.First();
            var methodInfo = GetMethod("Method");

            methodInfo.Name.ShouldEqual("Method");
            methodInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.MethodInfo.Method");
            methodInfo.Parent.ShouldEqual(classInfo);
        }

        [Fact]
        public void Expect_to_find_doc_comment()
        {
            var methodInfo = GetMethod("Method");
            methodInfo.DocComment.Summary.ShouldEqual("summary");
            methodInfo.DocComment.Returns.ShouldEqual("returns");
            methodInfo.DocComment.Parameters.First().Name.ShouldEqual("parameter");
            methodInfo.DocComment.Parameters.First().Description.ShouldEqual("param");
        }

        [Fact]
        public void Expect_to_find_attributes()
        {
            var methodInfo = GetMethod("Method");
            var attributeInfo = methodInfo.Attributes.First();

            methodInfo.Attributes.Count.ShouldEqual(1);
            attributeInfo.Name.ShouldEqual("AttributeInfo");
            attributeInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.AttributeInfoAttribute");
        }

        [Fact]
        public void Expect_to_find_parameters()
        {
            var methodInfo = GetMethod("Method");
            var parameterInfo = methodInfo.Parameters.First();

            methodInfo.Parameters.Count.ShouldEqual(1);
            parameterInfo.Name.ShouldEqual("parameter");
        }

        [Fact]
        public void Expect_to_find_parameter_attributes()
        {
            var methodInfo = GetMethod("Method");
            var parameterInfo = methodInfo.Parameters.First();
            var attributeInfo = parameterInfo.Attributes.First();

            parameterInfo.Attributes.Count.ShouldEqual(1);
            attributeInfo.Name.ShouldEqual("AttributeInfo");
            attributeInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.AttributeInfoAttribute");
        }

        [Fact]
        public void Expect_void_methods_to_return_void()
        {
            var methodInfo = GetMethod("Method");
            
            methodInfo.Type.FullName.ShouldEqual("System.Void");
            methodInfo.Type.Name.ShouldEqual("void");
            methodInfo.Type.OriginalName.ShouldEqual("Void");
            methodInfo.Type.IsEnum.ShouldBeFalse("IsEnum");
            methodInfo.Type.IsEnumerable.ShouldBeFalse("IsEnumerable");
            methodInfo.Type.IsGeneric.ShouldBeFalse("IsGeneric");
            methodInfo.Type.IsNullable.ShouldBeFalse("IsNullable");
            methodInfo.Type.IsTask.ShouldBeFalse("IsTask");
            methodInfo.Type.IsPrimitive.ShouldBeFalse("IsPrimitive");
        }

        [Fact]
        public void Expect_generic_methods_to_handle_generic_type_arguments()
        {
            var methodInfo = GetMethod("Generic");
            var genericTypeInfo = methodInfo.TypeParameters.First();
            var parameterTypeInfo = methodInfo.Parameters.First().Type;

            methodInfo.IsGeneric.ShouldBeTrue("IsGeneric");
            methodInfo.TypeParameters.Count.ShouldEqual(1);

            methodInfo.Type.Name.ShouldEqual("T");
            methodInfo.Type.FullName.ShouldEqual("T");
            genericTypeInfo.Name.ShouldEqual("T");
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
            methodInfo.TypeParameters.Count.ShouldEqual(0);

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
            methodInfo.TypeParameters.Count.ShouldEqual(1);

            methodInfo.Type.Name.ShouldEqual("T1");
            methodInfo.Type.FullName.ShouldEqual("T1");
            firstParameterTypeInfo.Name.ShouldEqual("T1");
            firstParameterTypeInfo.FullName.ShouldEqual("T1");
            secondParameterTypeInfo.Name.ShouldEqual("T");
            secondParameterTypeInfo.FullName.ShouldEqual("T");
        }

        [Fact]
        public void Expect_task_methods_to_return_void()
        {
            var methodInfo = GetMethod("Task");

            methodInfo.Type.FullName.ShouldEqual("System.Void");
            methodInfo.Type.Name.ShouldEqual("void");
            methodInfo.Type.OriginalName.ShouldEqual("Void");
            methodInfo.Type.IsEnum.ShouldBeFalse("IsEnum");
            methodInfo.Type.IsEnumerable.ShouldBeFalse("IsEnumerable");
            methodInfo.Type.IsGeneric.ShouldBeFalse("IsGeneric");
            methodInfo.Type.IsNullable.ShouldBeFalse("IsNullable");
            methodInfo.Type.IsTask.ShouldBeTrue("IsTask");
            methodInfo.Type.IsPrimitive.ShouldBeFalse("IsPrimitive");
        }

        [Fact]
        public void Expect_task_string_methods_to_return_string()
        {
            var methodInfo = GetMethod("TaskString");

            methodInfo.Type.FullName.ShouldEqual("System.String");
            methodInfo.Type.Name.ShouldEqual("string");
            methodInfo.Type.OriginalName.ShouldEqual("string");
            methodInfo.Type.IsEnum.ShouldBeFalse("IsEnum");
            methodInfo.Type.IsEnumerable.ShouldBeFalse("IsEnumerable");
            methodInfo.Type.IsGeneric.ShouldBeFalse("IsGeneric");
            methodInfo.Type.IsNullable.ShouldBeFalse("IsNullable");
            methodInfo.Type.IsTask.ShouldBeTrue("IsTask");
            methodInfo.Type.IsPrimitive.ShouldBeTrue("IsPrimitive");
        }

        [Fact]
        public void Expect_task_nullable_int_methods_to_return_int()
        {
            var methodInfo = GetMethod("TaskNullableInt");

            methodInfo.Type.FullName.ShouldEqual("System.Int32?");
            methodInfo.Type.Name.ShouldEqual("number");
            methodInfo.Type.OriginalName.ShouldEqual("int?");
            methodInfo.Type.IsEnum.ShouldBeFalse("IsEnum");
            methodInfo.Type.IsEnumerable.ShouldBeFalse("IsEnumerable");
            methodInfo.Type.IsGeneric.ShouldBeFalse("IsGeneric");
            methodInfo.Type.IsNullable.ShouldBeTrue("IsNullable");
            methodInfo.Type.IsTask.ShouldBeTrue("IsTask");
            methodInfo.Type.IsPrimitive.ShouldBeTrue("IsPrimitive");
        }

        [Fact]
        public void Expect_byteArray_parameter_primitive_not_to_throw_exception()
        {
            var methodInfo = GetMethod("ArrayParameter");

            methodInfo.Parameters.First().Type.IsPrimitive.ShouldEqual(true);
        }

        [Fact]
        public void Expect_parameter_without_default_values_not_to_have_a_default_value()
        {
            var methodInfo = GetMethod("Method");
            methodInfo.Parameters.First().HasDefaultValue.ShouldBeFalse();
            methodInfo.Parameters.First().DefaultValue.ShouldBeNull();
        }

        [Fact]
        public void Expect_parameter_default_values_to_return_correct_values()
        {
            var methodInfo = GetMethod("DefaultValueParameter");
            methodInfo.Parameters.First().HasDefaultValue.ShouldBeTrue();
            methodInfo.Parameters.First().DefaultValue.ShouldEqual("null");
            methodInfo.Parameters.First(p => p.name == "stringValue").DefaultValue.ShouldEqual("\"str\\\\ing\\\"quotes\\\"\"");
            methodInfo.Parameters.First(p => p.name == "boolValue").DefaultValue.ShouldEqual("true");
        }

        private Method GetMethod(string name)
        {
            var classInfo = fileInfo.Classes.First();
            var methodInfo = classInfo.Methods.First(p => p.Name == name);
            return methodInfo;
        }
    }
}