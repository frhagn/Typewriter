using System;
using System.Linq;
using Should;
using Typewriter.CodeModel;
using Xunit;

namespace Tests
{
    public class GenericClassTests : TestBase
    {
        private readonly File fileInfo = GetFile(@"Tests\CodeModel\GenericClassInfo.cs");

        [Fact]
        public void Info()
        {
            var classInfo = fileInfo.Classes.First();

            classInfo.Name.ShouldEqual("GenericClassInfo");
            classInfo.FullName.ShouldEqual("Tests.CodeModel.GenericClassInfo<T1, T2>");
            classInfo.Parent.ShouldEqual(fileInfo);

            classInfo.IsGeneric.ShouldBeTrue("Class IsGeneric");
            classInfo.GenericTypeArguments.Any().ShouldBeTrue("Class GenericTypeArguments");
        }

        [Fact]
        public void PrimitiveGenericProperties()
        {
            var classInfo = fileInfo.Classes.First();
            var property = classInfo.Properties.First(p => p.Name == "T11");

            property.Name.ShouldEqual("T11");
            property.FullName.ShouldEqual("Tests.CodeModel.GenericClassInfo<T1, T2>.T11");
            property.Parent.ShouldEqual(classInfo);
            
            property.HasGetter.ShouldBeTrue("HasGetter");
            property.HasSetter.ShouldBeTrue("HasSetter");

            property.IsEnum.ShouldBeFalse("IsEnum");
            property.IsEnumerable.ShouldBeFalse("IsEnumerable");
            property.IsGeneric.ShouldBeFalse("IsGeneric");
            property.IsNullable.ShouldBeFalse("IsNullable");
            property.IsPrimitive.ShouldBeFalse("IsPrimitive");

            property.Type.Name.ShouldEqual("T1");
            property.Type.FullName.ShouldEqual("T1");

            property.Type.IsEnum.ShouldBeFalse("IsEnum");
            property.Type.IsEnumerable.ShouldBeFalse("IsEnumerable");
            property.Type.IsGeneric.ShouldBeFalse("IsGeneric");
            property.Type.IsNullable.ShouldBeFalse("IsNullable");
            property.Type.IsPrimitive.ShouldBeFalse("IsPrimitive");
            property.Type.GenericTypeArguments.Any().ShouldBeFalse("GenericTypeArguments");
            property.Type.Parent.ShouldEqual(property);
        }

        [Fact]
        public void EnumerableGenericProperties()
        {
            var classInfo = fileInfo.Classes.First();
            var property = classInfo.Properties.First(p => p.Name == "EnumerableT11");

            property.Name.ShouldEqual("EnumerableT11");
            property.FullName.ShouldEqual("Tests.CodeModel.GenericClassInfo<T1, T2>.EnumerableT11");
            property.Parent.ShouldEqual(classInfo);

            property.Type.Name.ShouldEqual("IEnumerable");
            property.Type.FullName.ShouldEqual("System.Collections.Generic.IEnumerable<T1>");

            VerifyEnumerableProperty(property);
        }

        [Fact]
        public void ArrayGenericProperties()
        {
            var classInfo = fileInfo.Classes.First();
            var property = classInfo.Properties.First(p => p.Name == "T1Array1");

            property.Name.ShouldEqual("T1Array1");
            property.FullName.ShouldEqual("Tests.CodeModel.GenericClassInfo<T1, T2>.T1Array1");
            property.Parent.ShouldEqual(classInfo);

            property.Type.Name.ShouldEqual("ICollection");
            property.Type.FullName.ShouldEqual("System.Collections.Generic.ICollection<T1>");

            VerifyEnumerableProperty(property);
        }
        
        private static void VerifyEnumerableProperty(Property property)
        {
            property.HasGetter.ShouldBeTrue("HasGetter");
            property.HasSetter.ShouldBeTrue("HasSetter");

            property.IsEnum.ShouldBeFalse("IsEnum");
            property.IsEnumerable.ShouldBeTrue("IsEnumerable");
            property.IsGeneric.ShouldBeTrue("IsGeneric");
            property.IsNullable.ShouldBeFalse("IsNullable");
            property.IsPrimitive.ShouldBeFalse("IsPrimitive");

            property.Type.IsEnum.ShouldBeFalse("IsEnum");
            property.Type.IsEnumerable.ShouldBeTrue("IsEnumerable");
            property.Type.IsGeneric.ShouldBeTrue("IsGeneric");
            property.Type.IsNullable.ShouldBeFalse("IsNullable");
            property.Type.IsPrimitive.ShouldBeFalse("IsPrimitive");
            property.Type.GenericTypeArguments.Any().ShouldBeTrue("GenericTypeArguments");
            property.Type.Parent.ShouldEqual(property);

            var generic = property.Type.GenericTypeArguments.First();
            generic.Name.ShouldEqual("T1");
            generic.FullName.ShouldEqual("T1");
            generic.Parent.ShouldEqual(property.Type);
        }
    }
}