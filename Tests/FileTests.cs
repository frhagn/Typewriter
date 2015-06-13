using System;
using System.IO;
using System.Linq;
using Should;
using File = Typewriter.CodeModel.File;

namespace Tests
{
    public class FileTests : TestBase
    {
        private readonly File fileInfo = GetFile(@"Tests\CodeModel\FileInfo.cs");

        public void Info()
        {
            fileInfo.Name.ShouldEqual("FileInfo.cs");
            fileInfo.FullName.ShouldEqual(Path.Combine(SolutionDirectory, @"Tests\CodeModel\FileInfo.cs"));
        }

        public void Classes()
        {
            var classInfo = fileInfo.Classes.First();
            
            fileInfo.Classes.Count.ShouldEqual(1);
            classInfo.Name.ShouldEqual("PublicClass");
        }

        public void Enums()
        {
            var enumInfo = fileInfo.Enums.First();
            
            fileInfo.Enums.Count.ShouldEqual(1);
            enumInfo.Name.ShouldEqual("PublicEnum");
        }

        public void Interfaces()
        {
            var interfaceInfo = fileInfo.Interfaces.First();
            
            fileInfo.Interfaces.Count.ShouldEqual(1);
            interfaceInfo.Name.ShouldEqual("PublicInterface");
        }
    }
}