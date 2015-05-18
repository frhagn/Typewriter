using System;
using System.Linq;
using Should;
using Typewriter.CodeModel;

namespace Tests
{
    public class ClassTests : TestBase
    {
        private readonly File fileInfo = GetFile(@"Tests\CodeModel\ClassInfo.cs");

        public void Info()
        {
            var classInfo = fileInfo.Classes.First();

            classInfo.Name.ShouldEqual("Class1");
            classInfo.FullName.ShouldEqual("Tests.CodeModel.Class1");
            classInfo.Parent.ShouldEqual(fileInfo);
        }

        public void Attributes()
        {
            var classInfo = fileInfo.Classes.First();

            classInfo.Attributes.Count.ShouldEqual(1);

            classInfo.Attributes.First().Name.ShouldEqual("Test");
            classInfo.Attributes.First().FullName.ShouldEqual("Tests.CodeModel.TestAttribute");
            classInfo.Attributes.First().Value.ShouldEqual("classParameter");
        }

        #region Primitive properties

        public void BoolProperties()
        {
            TestPrimitiveProperty("Bool1", "Boolean");
        }

        public void CharProperties()
        {
            TestPrimitiveProperty("Char1", "Char");
        }

        public void StringProperties()
        {
            TestPrimitiveProperty("String1", "String");
        }

        public void ByteProperties()
        {
            TestPrimitiveProperty("Byte1", "Byte");
        }

        public void SbyteProperties()
        {
            TestPrimitiveProperty("Sbyte1", "SByte");
        }

        public void IntProperties()
        {
            TestPrimitiveProperty("Int1", "Int32");
        }

        public void UintProperties()
        {
            TestPrimitiveProperty("Uint1", "UInt32");
        }

        public void ShortProperties()
        {
            TestPrimitiveProperty("Short1", "Int16");
        }

        public void UshortProperties()
        {
            TestPrimitiveProperty("Ushort1", "UInt16");
        }

        public void LongProperties()
        {
            TestPrimitiveProperty("Long1", "Int64");
        }

        public void UlongProperties()
        {
            TestPrimitiveProperty("Ulong1", "UInt64");
        }

        public void FloatProperties()
        {
            TestPrimitiveProperty("Float1", "Single");
        }

        public void DoubleProperties()
        {
            TestPrimitiveProperty("Double1", "Double");
        }

        public void DecimalProperties()
        {
            TestPrimitiveProperty("Decimal1", "Decimal");
        }

        public void DateProperties()
        {
            TestPrimitiveProperty("DateTime1", "DateTime");
        }

        private void TestPrimitiveProperty(string name, string type)
        {
            var classInfo = fileInfo.Classes.First();
            var property = classInfo.Properties.First(p => p.Name == name);

            property.Name.ShouldEqual(name);
            property.FullName.ShouldEqual("Tests.CodeModel.Class1." + name);
            property.Parent.ShouldEqual(classInfo);

            property.HasGetter.ShouldBeTrue("HasGetter");
            property.HasSetter.ShouldBeTrue("HasSetter");

            property.IsEnum.ShouldBeFalse("IsEnum");
            property.IsEnumerable.ShouldBeFalse("IsEnumerable");
            property.IsGeneric.ShouldBeFalse("IsGeneric");
            property.IsNullable.ShouldBeFalse("IsNullable");
            property.IsPrimitive.ShouldBeTrue("IsPrimitive");

            property.Type.Name.ShouldEqual(type);
            property.Type.FullName.ShouldEqual("System." + type);

            property.Type.IsEnum.ShouldBeFalse("IsEnum");
            property.Type.IsEnumerable.ShouldBeFalse("IsEnumerable");
            property.Type.IsGeneric.ShouldBeFalse("IsGeneric");
            property.Type.IsNullable.ShouldBeFalse("IsNullable");
            property.Type.IsPrimitive.ShouldBeTrue("IsPrimitive");
            property.Type.GenericTypeArguments.Any().ShouldBeFalse("GenericTypeArguments");
            property.Type.Parent.ShouldEqual(property);
        }

        #endregion

        public void ObjectProperties()
        {
            var classInfo = fileInfo.Classes.First();
            var property = classInfo.Properties.First(p => p.Name == "Object1");

            property.Name.ShouldEqual("Object1");
            property.FullName.ShouldEqual("Tests.CodeModel.Class1.Object1");
            property.Parent.ShouldEqual(classInfo);

            property.HasGetter.ShouldBeTrue("HasGetter");
            property.HasSetter.ShouldBeTrue("HasSetter");

            property.IsEnum.ShouldBeFalse("IsEnum");
            property.IsEnumerable.ShouldBeFalse("IsEnumerable");
            property.IsGeneric.ShouldBeFalse("IsGeneric");
            property.IsNullable.ShouldBeFalse("IsNullable");
            property.IsPrimitive.ShouldBeFalse("IsPrimitive");

            property.Type.Name.ShouldEqual("Object");
            property.Type.FullName.ShouldEqual("System.Object");

            property.Type.IsEnum.ShouldBeFalse("IsEnum");
            property.Type.IsEnumerable.ShouldBeFalse("IsEnumerable");
            property.Type.IsGeneric.ShouldBeFalse("IsGeneric");
            property.Type.IsNullable.ShouldBeFalse("IsNullable");
            property.Type.IsPrimitive.ShouldBeFalse("IsPrimitive");
            property.Type.GenericTypeArguments.Any().ShouldBeFalse("GenericTypeArguments");
            property.Type.Parent.ShouldEqual(property);
        }

        public void DefinedClassProperties()
        {
            var classInfo = fileInfo.Classes.First();
            var property = classInfo.Properties.First(p => p.Name == "Class11");

            property.Name.ShouldEqual("Class11");
            property.FullName.ShouldEqual("Tests.CodeModel.Class1.Class11");
            property.Parent.ShouldEqual(classInfo);

            property.HasGetter.ShouldBeTrue("HasGetter");
            property.HasSetter.ShouldBeTrue("HasSetter");

            property.IsEnum.ShouldBeFalse("IsEnum");
            property.IsEnumerable.ShouldBeFalse("IsEnumerable");
            property.IsGeneric.ShouldBeFalse("IsGeneric");
            property.IsNullable.ShouldBeFalse("IsNullable");
            property.IsPrimitive.ShouldBeFalse("IsPrimitive");

            property.Type.Name.ShouldEqual("Class1");
            property.Type.FullName.ShouldEqual("Tests.CodeModel.Class1");

            property.Type.IsEnum.ShouldBeFalse("IsEnum");
            property.Type.IsEnumerable.ShouldBeFalse("IsEnumerable");
            property.Type.IsGeneric.ShouldBeFalse("IsGeneric");
            property.Type.IsNullable.ShouldBeFalse("IsNullable");
            property.Type.IsPrimitive.ShouldBeFalse("IsPrimitive");
            property.Type.GenericTypeArguments.Any().ShouldBeFalse("GenericTypeArguments");
            property.Type.Parent.ShouldEqual(property);

            property.Type.Attributes.Count.ShouldEqual(1);
            property.Type.Properties.Any().ShouldBeTrue();
            property.Type.Methods.Any().ShouldBeFalse();

            var typeProperty = property.Type.Properties.First(p => p.Name == "Class11");
            typeProperty.Name.ShouldEqual("Class11");
            typeProperty.FullName.ShouldEqual("Tests.CodeModel.Class1.Class11");
            typeProperty.Parent.ShouldEqual(property.Type);
        }

        public void DefinedEnumProperties()
        {
            var classInfo = fileInfo.Classes.First();
            var property = classInfo.Properties.First(p => p.Name == "Enum11");

            property.Name.ShouldEqual("Enum11");
            property.FullName.ShouldEqual("Tests.CodeModel.Class1.Enum11");
            property.Parent.ShouldEqual(classInfo);

            property.HasGetter.ShouldBeTrue("HasGetter");
            property.HasSetter.ShouldBeTrue("HasSetter");

            property.IsEnum.ShouldBeTrue("IsEnum");
            property.IsEnumerable.ShouldBeFalse("IsEnumerable");
            property.IsGeneric.ShouldBeFalse("IsGeneric");
            property.IsNullable.ShouldBeFalse("IsNullable");
            property.IsPrimitive.ShouldBeTrue("IsPrimitive");
        }

        public void EnumerableDefinedClassProperties()
        {
            var classInfo = fileInfo.Classes.First();
            var property = classInfo.Properties.First(p => p.Name == "IEnumerableClass11");

            property.Name.ShouldEqual("IEnumerableClass11");
            property.FullName.ShouldEqual("Tests.CodeModel.Class1.IEnumerableClass11");
            property.Parent.ShouldEqual(classInfo);

            property.HasGetter.ShouldBeTrue("HasGetter");
            property.HasSetter.ShouldBeTrue("HasSetter");

            property.IsEnum.ShouldBeFalse("IsEnum");
            property.IsEnumerable.ShouldBeTrue("IsEnumerable");
            property.IsGeneric.ShouldBeTrue("IsGeneric");
            property.IsNullable.ShouldBeFalse("IsNullable");
            property.IsPrimitive.ShouldBeFalse("IsPrimitive");

            property.Type.Name.ShouldEqual("IEnumerable");
            property.Type.FullName.ShouldEqual("System.Collections.Generic.IEnumerable<Tests.CodeModel.Class1>");

            property.Type.IsEnum.ShouldBeFalse("IsEnum");
            property.Type.IsEnumerable.ShouldBeTrue("IsEnumerable");
            property.Type.IsGeneric.ShouldBeTrue("IsGeneric");
            property.Type.IsNullable.ShouldBeFalse("IsNullable");
            property.Type.IsPrimitive.ShouldBeFalse("IsPrimitive");
            property.Type.GenericTypeArguments.Any().ShouldBeTrue("GenericTypeArguments");
            property.Type.Parent.ShouldEqual(property);

            var generic = property.Type.GenericTypeArguments.First();
            generic.Name.ShouldEqual("Class1");
            generic.FullName.ShouldEqual("Tests.CodeModel.Class1");
            generic.Parent.ShouldEqual(property.Type);

            generic.Attributes.Count.ShouldEqual(1);
            generic.Properties.Any().ShouldBeTrue();
            generic.Methods.Any().ShouldBeFalse();

            var typeProperty = generic.Properties.First(p => p.Name == "Class11");
            typeProperty.Name.ShouldEqual("Class11");
            typeProperty.FullName.ShouldEqual("Tests.CodeModel.Class1.Class11");
            typeProperty.Parent.ShouldEqual(generic);
        }

        
    }
}