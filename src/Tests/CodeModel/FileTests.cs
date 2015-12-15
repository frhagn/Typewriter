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
            fileInfo.Classes.Count.ShouldEqual(2);

            var classInfo1 = fileInfo.Classes.First();
            classInfo1.Name.ShouldEqual("PublicClassNoNamespace");

            var classInfo2 = fileInfo.Classes.Last();
            classInfo2.Name.ShouldEqual("PublicClass");
        }

        [Fact]
        public void Expect_to_find_public_delegates()
        {
            fileInfo.Delegates.Count.ShouldEqual(2);

            var delegateInfo1 = fileInfo.Delegates.First();
            delegateInfo1.Name.ShouldEqual("PublicDelegateNoNamespace");

            var delegateInfo2 = fileInfo.Delegates.Last();
            delegateInfo2.Name.ShouldEqual("PublicDelegate");
        }

        [Fact]
        public void Expect_to_find_public_enums()
        {
            fileInfo.Enums.Count.ShouldEqual(2);

            var enumInfo1 = fileInfo.Enums.First();
            enumInfo1.Name.ShouldEqual("PublicEnumNoNamespace");

            var enumInfo2 = fileInfo.Enums.Last();
            enumInfo2.Name.ShouldEqual("PublicEnum");
        }

        [Fact]
        public void Expect_to_find_public_interfaces()
        {
            fileInfo.Interfaces.Count.ShouldEqual(2);

            var interfaceInfo1 = fileInfo.Interfaces.First();
            interfaceInfo1.Name.ShouldEqual("PublicInterfaceNoNamespace");

            var interfaceInfo2 = fileInfo.Interfaces.Last();
            interfaceInfo2.Name.ShouldEqual("PublicInterface");
        }
    }
}