using System.IO;
using System.Linq;
using Should;
using Typewriter.Tests.TestInfrastructure;
using Xunit;
using File = Typewriter.CodeModel.File;

namespace Typewriter.Tests.CodeModel
{
    [Trait("CodeModel", "Files"), Collection(nameof(CodeDomFixture))]
    public class CodeDomFileTests : FileTests
    {
        public CodeDomFileTests(CodeDomFixture fixture) : base(fixture)
        {
        }
    }

    [Trait("CodeModel", "Files"), Collection(nameof(RoslynFixture))]
    public class RoslynFileTests : FileTests
    {
        public RoslynFileTests(RoslynFixture fixture) : base(fixture)
        {
        }
    }

    public abstract class FileTests : TestBase
    {
        private readonly File fileInfo;

        protected FileTests(ITestFixture fixture) : base(fixture)
        {
            fileInfo = GetFile(@"Tests\CodeModel\Support\FileInfo.cs");
        }

        [Fact]
        public void Expect_name_to_match_filename()
        {
            fileInfo.Name.ShouldEqual("FileInfo.cs");
            fileInfo.FullName.ShouldEqual(Path.Combine(SolutionDirectory, @"Tests\CodeModel\Support\FileInfo.cs"));
        }

        [Fact]
        public void Expect_to_find_public_classes()
        {
            fileInfo.Classes.Count.ShouldEqual(1);

            var classInfo = fileInfo.Classes.First();
            classInfo.Name.ShouldEqual("PublicClass");
        }

        [Fact]
        public void Expect_to_find_public_delegates()
        {
            fileInfo.Delegates.Count.ShouldEqual(1);
            
            var delegateInfo = fileInfo.Delegates.First();
            delegateInfo.Name.ShouldEqual("PublicDelegate");
        }

        [Fact]
        public void Expect_to_find_public_enums()
        {
            fileInfo.Enums.Count.ShouldEqual(1);
            
            var enumInfo = fileInfo.Enums.First();
            enumInfo.Name.ShouldEqual("PublicEnum");
        }

        [Fact]
        public void Expect_to_find_public_interfaces()
        {
            fileInfo.Interfaces.Count.ShouldEqual(1);
            
            var interfaceInfo = fileInfo.Interfaces.First();
            interfaceInfo.Name.ShouldEqual("PublicInterface");
        }
    }
}