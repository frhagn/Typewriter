using System.Linq;
using Should;
using Typewriter.CodeModel;
using Typewriter.Tests.TestInfrastructure;
using Xunit;

namespace Typewriter.Tests.CodeModel
{
    [Trait("CodeModel", "Constants"), Collection(nameof(CodeDomFixture))]
    public class CodeDomConstantTests : ConstantTests
    {
        public CodeDomConstantTests(CodeDomFixture fixture) : base(fixture)
        {
        }
    }
    
    [Trait("CodeModel", "Constants"), Collection(nameof(RoslynFixture))]
    public class RoslynConstantTests : ConstantTests
    {
        public RoslynConstantTests(RoslynFixture fixture) : base(fixture)
        {
        }
    }
    
    public abstract class ConstantTests : TestBase
    {
        private readonly File fileInfo;

        protected ConstantTests(ITestFixture fixture) : base(fixture)
        {
            fileInfo = GetFile(@"Tests\CodeModel\Support\ConstantInfo.cs");
        }

        [Fact]
        public void Expect_name_to_match_cosntant_name()
        {
            var classInfo = fileInfo.Classes.First();
            var constantInfo = classInfo.Constants.First();

            constantInfo.Name.ShouldEqual("StringField");
            constantInfo.FullName.ShouldEqual("Typewriter.Tests.CodeModel.Support.ConstantInfo.StringField");
            constantInfo.Parent.ShouldEqual(classInfo);
        }

        [Fact]
        public void Expect_to_find_doc_comment()
        {
            var classInfo = fileInfo.Classes.First();
            var constantInfo = classInfo.Constants.First();
            constantInfo.DocComment.Summary.ShouldEqual("summary");
        }

        [Fact]
        public void Expect_value_not_empty()
        {
            var classInfo = fileInfo.Classes.First();
            var constantInfo = classInfo.Constants.First();

            constantInfo.Value.ShouldEqual("test\"quotes\"");
        }

        [Fact]
        public void Expect_value_to_be_empty()
        {
            var classInfo = fileInfo.Classes.First();
            var constantInfo = classInfo.Constants.Skip(1).First();

            constantInfo.Value.ShouldEqual(string.Empty);
        }

        [Fact]
        public void Expect_value_to_be_number()
        {
            var classInfo = fileInfo.Classes.First();
            var constantInfo = classInfo.Constants.Skip(2).First();

            constantInfo.Value.ShouldEqual("123");
        }
    }
}