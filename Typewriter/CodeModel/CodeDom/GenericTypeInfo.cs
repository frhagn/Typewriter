using System;
using System.Collections.Generic;

namespace Typewriter.CodeModel.CodeDom
{
    internal class GenericTypeInfo : Type
    {
        private readonly string fullName;
        private readonly object parent;
        private readonly FileInfo file;

        public GenericTypeInfo(string fullName, object parent, FileInfo file)
        {
            this.fullName = fullName;
            this.parent = parent;
            this.file = file;
        }

        public ICollection<Attribute> Attributes => new Attribute[0];
        public ICollection<Constant> Constants => new Constant[0];
        public ICollection<Field> Fields => new Field[0];
        public Class BaseClass => null;
        public string FullName => fullName;
        public IEnumerable<Type> GenericTypeArguments => new Type[0];
        public ICollection<Interface> Interfaces => new Interface[0];
        public bool IsEnum => false;
        public bool IsEnumerable => false;
        public bool IsGeneric => false;
        public bool IsNullable => false;
        public bool IsPrimitive => false;
        public ICollection<Method> Methods => new Method[0];
        public string Name => fullName;
        public string Namespace => null;
        public object Parent => parent;
        public ICollection<Property> Properties => new Property[0];

        public override string ToString()
        {
            return fullName;
        }
    }
}