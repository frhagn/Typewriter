using System;
using System.Collections.Generic;

namespace Typewriter.CodeModel.CodeDom
{
    internal class GenericTypeInfo : Type
    {
        private string fullName;
        private object parent;
        private FileInfo file;

        public GenericTypeInfo(string fullName, object parent, FileInfo file)
        {
            this.fullName = fullName;
            this.parent = parent;
            this.file = file;
        }

        public ICollection<Attribute> Attributes
        {
            get { return new Attribute[0]; }
        }

        public ICollection<Constant> Constants
        {
            get { return new Constant[0]; }
        }

        public ICollection<Field> Fields
        {
            get { return new Field[0]; }
        }

        public string FullName
        {
            get { return fullName; }
        }

        public IEnumerable<Type> GenericTypeArguments
        {
            get { return new Type[0]; }
        }

        public ICollection<Interface> Interfaces
        {
            get { return new Interface[0]; }
        }

        public bool IsEnum
        {
            get { return false; }
        }

        public bool IsEnumerable
        {
            get { return false; }
        }

        public bool IsGeneric
        {
            get { return false; }
        }

        public bool IsNullable
        {
            get { return false; }
        }

        public bool IsPrimitive
        {
            get { return false; }
        }

        public ICollection<Method> Methods
        {
            get { return new Method[0]; }
        }

        public string Name
        {
            get { return fullName; }
        }

        public string Namespace
        {
            get { return null; }
        }

        public object Parent
        {
            get { return parent; }
        }

        public ICollection<Property> Properties
        {
            get { return new Property[0]; }
        }
    }
}