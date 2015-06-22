using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Typewriter.CodeModel.CodeDom
{
    internal class GenericTypeInfo : Type
    {
        private readonly string fullName;
        private readonly Item parent;

        public GenericTypeInfo(string fullName, Item parent)
        {
            this.fullName = fullName;
            this.parent = parent;
        }

        public Item Parent => parent;
        public ICollection<Attribute> Attributes => new Attribute[0];
        public ICollection<Constant> Constants => new Constant[0];
        public ICollection<Field> Fields => new Field[0];
        public Class BaseClass => null;
        public Class ContainingClass => null;
        public string FullName => fullName;
        public ICollection<Type> GenericTypeArguments => new Type[0];
        public ICollection<Interface> Interfaces => new Interface[0];
        public bool IsEnum => false;
        public bool IsEnumerable => false;
        public bool IsGeneric => false;
        public bool IsNullable => false;
        public bool IsPrimitive => false;
        public ICollection<Method> Methods => new Method[0];
        public string Name => fullName;
        public string Namespace => null;
        public ICollection<Property> Properties => new Property[0];
        public ICollection<Class> NestedClasses => new Class[0];
        public ICollection<Enum> NestedEnums => new Enum[0];
        public ICollection<Interface> NestedInterfaces => new Interface[0];

        internal static IEnumerable<Type> FromFullName(string fullName, Item parent)
        {
            return ExtractGenericTypeNames(fullName).Select(n => new GenericTypeInfo(n, parent));
        }

        internal static IEnumerable<string> ExtractGenericTypeNames(string name)
        {
            var list = new List<string>();
            var start = name.IndexOf("<", StringComparison.Ordinal);
            var end = name.LastIndexOf(">", StringComparison.Ordinal) - (start + 1);

            if (start < 0)
            {
                return list;
            }

            var arguments = name.Substring(start + 1, end);

            var current = new StringBuilder();
            var level = 0;
            foreach (var character in arguments)
            {
                if (character == ',' && level == 0)
                {
                    list.Add(current.ToString());
                    current = new StringBuilder();
                }
                else
                {
                    if (character == '<')
                        level++;
                    else if (character == '>')
                        level--;

                    current.Append(character);
                }
            }

            if (current.Length > 0)
                list.Add(current.ToString());

            return list;
        }
    }
}