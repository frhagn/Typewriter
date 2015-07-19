using System.Collections.Generic;
using System.IO;
using System.Linq;
using Should;
using Typewriter.Metadata.CodeDom;
using Typewriter.Metadata.Providers;
using Typewriter.Tests.TestInfrastructure;
using Xunit;
using File = Typewriter.CodeModel.File;

namespace Typewriter.Tests.CodeModel
{
    [Trait("Files", "CodeDom")]
    public class CodeDomFileTests : FileTests<CodeDomMetadataProvider>
    {
    }

    //[Trait("Files", "Roslyn")]
    //public class RoslynFileTests : FileTests<RoslynProviderStub>
    //{
    //}

    public abstract class FileTests<T> : TestBase<T> where T : IMetadataProvider, new()
    {
        private readonly File fileInfo;

        protected FileTests()
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