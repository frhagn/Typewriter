using System.Linq;
using Should;
using Typewriter.CodeModel;
using Typewriter.Tests.TestInfrastructure;
using Xunit;

namespace Typewriter.Tests.CodeModel
{
    [Trait("CodeModel", "Delegates"), Collection(nameof(CodeDomFixture))]
    public class CodeDomDelegateTests : DelegateTests
    {
        public CodeDomDelegateTests(CodeDomFixture fixture) : base(fixture)
        {
        }
    }

    [Trait("CodeModel", "Delegates"), Collection(nameof(RoslynFixture))]
    public class RoslynDelegateTests : DelegateTests
    {
        public RoslynDelegateTests(RoslynFixture fixture) : base(fixture)
        {
        }
    }

    public abstract class DelegateTests : TestBase
    {
        private readonly File fileInfo;

        protected DelegateTests(ITestFixture fixture) : base(fixture)
        {
            fileInfo = GetFile(@"Tests\CodeModel\Support\DelegateInfo.cs");
        }

        [Fact]
        public void Expect_name_to_match_delegate_name()
        {
            var classInfo = fileInfo.Classes.First();
            var delegateInfo = classInfo.Delegates.First(p => p.Name == "Delegate");

            delegateInfo.Name.ShouldEqual("Delegate");
            delegateInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.DelegateInfo.Delegate");
            delegateInfo.Parent.ShouldEqual(classInfo);
        }

        [Fact]
        public void Expect_to_find_doc_comment()
        {
            var classInfo = fileInfo.Classes.First();
            var delegateInfo = classInfo.Delegates.First(p => p.Name == "Delegate");
            delegateInfo.DocComment.Summary.ShouldEqual("summary");
            delegateInfo.DocComment.Parameters.First().Name.ShouldEqual("parameter");
            delegateInfo.DocComment.Parameters.First().Description.ShouldEqual("param");
        }

        [Fact]
        public void Expect_to_find_attributes()
        {
            var classInfo = fileInfo.Classes.First();
            var delegateInfo = classInfo.Delegates.First(p => p.Name == "Delegate");
            var attributeInfo = delegateInfo.Attributes.First();

            delegateInfo.Attributes.Count.ShouldEqual(1);
            attributeInfo.Name.ShouldEqual("AttributeInfo");
            attributeInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.AttributeInfoAttribute");
        }

        [Fact]
        public void Expect_to_find_parameters()
        {
            var classInfo = fileInfo.Classes.First();
            var delegateInfo = classInfo.Delegates.First(p => p.Name == "Delegate");
            var parameterInfo = delegateInfo.Parameters.First();

            delegateInfo.Parameters.Count.ShouldEqual(1);
            parameterInfo.Name.ShouldEqual("parameter");
        }

        [Fact]
        public void Expect_to_find_parameter_attributes()
        {
            var classInfo = fileInfo.Classes.First();
            var delegateInfo = classInfo.Delegates.First(p => p.Name == "Delegate");
            var parameterInfo = delegateInfo.Parameters.First();
            var attributeInfo = parameterInfo.Attributes.First();

            parameterInfo.Attributes.Count.ShouldEqual(1);
            attributeInfo.Name.ShouldEqual("AttributeInfo");
            attributeInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.AttributeInfoAttribute");
        }

        [Fact]
        public void Expect_void_delegates_to_return_void()
        {
            var classInfo = fileInfo.Classes.First();
            var delegateInfo = classInfo.Delegates.First(p => p.Name == "Delegate");
            
            delegateInfo.Type.FullName.ShouldEqual("System.Void");
            delegateInfo.Type.Name.ShouldEqual("void");
            delegateInfo.Type.OriginalName.ShouldEqual("Void");
            delegateInfo.Type.IsEnum.ShouldBeFalse("IsEnum");
            delegateInfo.Type.IsEnumerable.ShouldBeFalse("IsEnumerable");
            delegateInfo.Type.IsGeneric.ShouldBeFalse("IsGeneric");
            delegateInfo.Type.IsNullable.ShouldBeFalse("IsNullable");
            delegateInfo.Type.IsTask.ShouldBeFalse("IsTask");
            delegateInfo.Type.IsPrimitive.ShouldBeFalse("IsPrimitive");
        }

        [Fact]
        public void Expect_generic_delegates_to_handle_generic_type_arguments()
        {
            var classInfo = fileInfo.Classes.First();
            var delegateInfo = classInfo.Delegates.First(p => p.Name == "Generic");
            var genericTypeInfo = delegateInfo.TypeParameters.First();
            var parameterTypeInfo = delegateInfo.Parameters.First().Type;

            delegateInfo.IsGeneric.ShouldBeTrue("IsGeneric");
            delegateInfo.TypeParameters.Count.ShouldEqual(1);

            delegateInfo.Type.Name.ShouldEqual("T");
            delegateInfo.Type.FullName.ShouldEqual("T");
            genericTypeInfo.Name.ShouldEqual("T");
            parameterTypeInfo.Name.ShouldEqual("T");
            parameterTypeInfo.FullName.ShouldEqual("T");
        }

        [Fact]
        public void Expect_delegates_to_handle_generic_type_arguments_from_class()
        {
            var classInfo = fileInfo.Classes.First(c => c.Name == "GenericDelegateInfo");
            var delegateInfo = classInfo.Delegates.First(p => p.Name == "Delegate");
            var parameterTypeInfo = delegateInfo.Parameters.First().Type;

            delegateInfo.IsGeneric.ShouldBeFalse("IsGeneric");
            delegateInfo.TypeParameters.Count.ShouldEqual(0);

            delegateInfo.Type.Name.ShouldEqual("T");
            delegateInfo.Type.FullName.ShouldEqual("T");
            parameterTypeInfo.Name.ShouldEqual("T");
            parameterTypeInfo.FullName.ShouldEqual("T");
        }

        [Fact]
        public void Expect_generic_delegates_to_handle_generic_type_arguments_from_class_and_delegate()
        {
            var classInfo = fileInfo.Classes.First(c => c.Name == "GenericDelegateInfo");
            var delegateInfo = classInfo.Delegates.First(p => p.Name == "Generic");
            var firstParameterTypeInfo = delegateInfo.Parameters.First(p => p.Name == "parameter1").Type;
            var secondParameterTypeInfo = delegateInfo.Parameters.First(p => p.Name == "parameter2").Type;

            delegateInfo.IsGeneric.ShouldBeTrue("IsGeneric");
            delegateInfo.TypeParameters.Count.ShouldEqual(1);

            delegateInfo.Type.Name.ShouldEqual("T1");
            delegateInfo.Type.FullName.ShouldEqual("T1");
            firstParameterTypeInfo.Name.ShouldEqual("T1");
            firstParameterTypeInfo.FullName.ShouldEqual("T1");
            secondParameterTypeInfo.Name.ShouldEqual("T");
            secondParameterTypeInfo.FullName.ShouldEqual("T");
        }

        [Fact]
        public void Expect_task_delegates_to_return_void()
        {
            var classInfo = fileInfo.Classes.First();
            var delegateInfo = classInfo.Delegates.First(p => p.Name == "Task");

            delegateInfo.Type.FullName.ShouldEqual("System.Void");
            delegateInfo.Type.Name.ShouldEqual("void");
            delegateInfo.Type.OriginalName.ShouldEqual("Void");
            delegateInfo.Type.IsEnum.ShouldBeFalse("IsEnum");
            delegateInfo.Type.IsEnumerable.ShouldBeFalse("IsEnumerable");
            delegateInfo.Type.IsGeneric.ShouldBeFalse("IsGeneric");
            delegateInfo.Type.IsNullable.ShouldBeFalse("IsNullable");
            delegateInfo.Type.IsTask.ShouldBeTrue("IsTask");
            delegateInfo.Type.IsPrimitive.ShouldBeFalse("IsPrimitive");
        }

        [Fact]
        public void Expect_task_string_delegates_to_return_string()
        {
            var classInfo = fileInfo.Classes.First();
            var delegateInfo = classInfo.Delegates.First(p => p.Name == "TaskString");

            delegateInfo.Type.FullName.ShouldEqual("System.String");
            delegateInfo.Type.Name.ShouldEqual("string");
            delegateInfo.Type.OriginalName.ShouldEqual("string");
            delegateInfo.Type.IsEnum.ShouldBeFalse("IsEnum");
            delegateInfo.Type.IsEnumerable.ShouldBeFalse("IsEnumerable");
            delegateInfo.Type.IsGeneric.ShouldBeFalse("IsGeneric");
            delegateInfo.Type.IsNullable.ShouldBeFalse("IsNullable");
            delegateInfo.Type.IsTask.ShouldBeTrue("IsTask");
            delegateInfo.Type.IsPrimitive.ShouldBeTrue("IsPrimitive");
        }

        [Fact]
        public void Expect_task_nullable_int_delegates_to_return_int()
        {
            var classInfo = fileInfo.Classes.First();
            var delegateInfo = classInfo.Delegates.First(p => p.Name == "TaskNullableInt");

            delegateInfo.Type.FullName.ShouldEqual("System.Int32?");
            delegateInfo.Type.Name.ShouldEqual("number");
            delegateInfo.Type.OriginalName.ShouldEqual("int?");
            delegateInfo.Type.IsEnum.ShouldBeFalse("IsEnum");
            delegateInfo.Type.IsEnumerable.ShouldBeFalse("IsEnumerable");
            delegateInfo.Type.IsGeneric.ShouldBeFalse("IsGeneric");
            delegateInfo.Type.IsNullable.ShouldBeTrue("IsNullable");
            delegateInfo.Type.IsTask.ShouldBeTrue("IsTask");
            delegateInfo.Type.IsPrimitive.ShouldBeTrue("IsPrimitive");
        }
    }
}