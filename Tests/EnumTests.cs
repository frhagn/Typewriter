using System;
using System.Linq;
using Should;
using Typewriter.CodeModel;

namespace Tests
{
    public class EnumTests : TestBase
    {
        private readonly File fileInfo = GetFile(@"Tests\CodeModel\EnumInfo.cs");

        public void Info()
        {
            var enumInfo = fileInfo.Enums.First();

            enumInfo.Name.ShouldEqual("Enum1");
            enumInfo.FullName.ShouldEqual("Tests.CodeModel.Enum1");
            enumInfo.Parent.ShouldEqual(fileInfo);
        }

        public void Attributes()
        {
            var enumInfo = fileInfo.Enums.First();

            enumInfo.Attributes.Count.ShouldEqual(2);

            enumInfo.Attributes.First().Name.ShouldEqual("Test");
            enumInfo.Attributes.First().FullName.ShouldEqual("Tests.CodeModel.TestAttribute");
            enumInfo.Attributes.First().Value.ShouldEqual("enumParameter");

            enumInfo.Attributes.Skip(1).First().Name.ShouldEqual("Flags");
            enumInfo.Attributes.Skip(1).First().FullName.ShouldEqual("System.FlagsAttribute");
        }

        public void Values()
        {
            var enumInfo = fileInfo.Enums.First();

            enumInfo.Values.Count.ShouldEqual(3);

            enumInfo.Values.First().Name.ShouldEqual("ValueA");
            enumInfo.Values.First().FullName.ShouldEqual("Tests.CodeModel.Enum1.ValueA");
            enumInfo.Values.First().Value.ShouldEqual(0);
            enumInfo.Values.First().Attributes.Count.ShouldEqual(0);
            enumInfo.Values.First().Parent.ShouldEqual(enumInfo);

            enumInfo.Values.Skip(1).First().Name.ShouldEqual("ValueB");
            enumInfo.Values.Skip(1).First().FullName.ShouldEqual("Tests.CodeModel.Enum1.ValueB");
            enumInfo.Values.Skip(1).First().Value.ShouldEqual(2);
            enumInfo.Values.Skip(1).First().Attributes.Count.ShouldEqual(1);
            enumInfo.Values.Skip(1).First().Attributes.First().Name.ShouldEqual("Test");
            enumInfo.Values.Skip(1).First().Parent.ShouldEqual(enumInfo);

            enumInfo.Values.Skip(2).First().Name.ShouldEqual("ValueC");
            enumInfo.Values.Skip(2).First().FullName.ShouldEqual("Tests.CodeModel.Enum1.ValueC");
            enumInfo.Values.Skip(2).First().Value.ShouldEqual(1);
            enumInfo.Values.Skip(2).First().Parent.ShouldEqual(enumInfo);
        }
    }
}