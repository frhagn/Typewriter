using System.Linq;
using Microsoft.VisualStudio.Sdk.TestFramework;
using Should;
using Typewriter.CodeModel;
using Typewriter.Tests.TestInfrastructure;
using Xunit;

namespace Typewriter.Tests.CodeModel
{
    [Trait("CodeModel", "Events"), Collection(nameof(CodeDomFixture))]
    public class CodeDomEventTests : EventTests
    {
        public CodeDomEventTests(CodeDomFixture fixture, GlobalServiceProvider sp) : base(fixture, sp)
        {
        }
    }

    [Trait("CodeModel", "Events"), Collection(nameof(RoslynFixture))]
    public class RoslynEventTests : EventTests
    {
        public RoslynEventTests(RoslynFixture fixture, GlobalServiceProvider sp) : base(fixture, sp)
        {
        }
    }

    public abstract class EventTests : TestBase
    {
        private readonly File fileInfo;

        protected EventTests(ITestFixture fixture, GlobalServiceProvider sp) : base(fixture, sp)
        {
            fileInfo = GetFile(@"Tests\CodeModel\Support\EventInfo.cs");
        }

        [Fact]
        public void Expect_name_to_match_property_name()
        {
            var classInfo = fileInfo.Classes.First();
            var enumInfo = classInfo.Events.First(p => p.Name == "DelegateEvent");
            
            enumInfo.Name.ShouldEqual("DelegateEvent");
            enumInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.EventInfo.DelegateEvent");
            enumInfo.Parent.ShouldEqual(classInfo);
        }

        [Fact]
        public void Expect_to_find_doc_comment()
        {
            var classInfo = fileInfo.Classes.First();
            var enumInfo = classInfo.Events.First(p => p.Name == "DelegateEvent");
            enumInfo.DocComment.Summary.ShouldEqual("summary");
        }

        [Fact]
        public void Expect_to_find_attributes()
        {
            var classInfo = fileInfo.Classes.First();
            var enumInfo = classInfo.Events.First(p => p.Name == "DelegateEvent");
            var attributeInfo = enumInfo.Attributes.First();

            enumInfo.Attributes.Count.ShouldEqual(1);
            attributeInfo.Name.ShouldEqual("AttributeInfo");
            attributeInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.AttributeInfoAttribute");
        }

        [Fact]
        public void Expect_generic_delegate_type_type_to_match_generic_argument()
        {
            var classInfo = fileInfo.Classes.First();
            var eventInfo = classInfo.Events.First(e => e.Name == "GenericDelegateEvent");
            var typeInfo = eventInfo.Type;

            typeInfo.Name.ShouldEqual("GenericDelegate<string>");

            typeInfo.TypeArguments.Count.ShouldEqual(1);
            typeInfo.TypeParameters.Count.ShouldEqual(1);

            typeInfo.TypeArguments.First().Name.ShouldEqual("string");
            if (isRoslyn)
            {
                typeInfo.TypeParameters.First().Name.ShouldEqual("T");
            }
        }
    }
}