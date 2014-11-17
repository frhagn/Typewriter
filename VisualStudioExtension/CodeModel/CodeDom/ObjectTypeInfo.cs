//using System;
//using System.Collections.Generic;

//namespace Typewriter.CodeModel.CodeDom
//{
//    public class ObjectTypeInfo : ITypeInfo
//    {
//        public string Name
//        {
//            get { return "object"; }
//        }

//        public string FullName
//        {
//            get { return "System.Object"; }
//        }

//        public bool IsPrimitive
//        {
//            get { return false; }
//        }

//        public bool IsEnumerable
//        {
//            get { return false; }
//        }

//        public bool IsEnum
//        {
//            get { return false; }
//        }

//        public bool IsGeneric
//        {
//            get { return false; }
//        }

//        public bool IsNullable
//        {
//            get { return false; }
//        }

//        public ICollection<IConstantInfo> Constants
//        {
//            get { return new List<IConstantInfo>(); }
//        }

//        public IEnumerable<IFieldInfo> Fields
//        {
//            get { return new List<IFieldInfo>(); }
//        }

//        public ICollection<IAttributeInfo> Attributes
//        {
//            get { return new List<IAttributeInfo>(); }
//        }

//        public IEnumerable<IInterfaceInfo> Interfaces
//        {
//            get { return new List<IInterfaceInfo>(); }
//        }

//        public IEnumerable<IPropertyInfo> Properties
//        {
//            get { return new List<IPropertyInfo>(); }
//        }

//        public IEnumerable<IMethodInfo> Methods
//        {
//            get { return new List<IMethodInfo>(); }
//        }

//        public IEnumerable<ITypeInfo> GenericTypeArguments
//        {
//            get { return new List<ITypeInfo>(); }
//        }
//    }
//}