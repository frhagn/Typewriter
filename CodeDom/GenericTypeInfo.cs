using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typewriter.CodeModel.Collections;

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
        public AttributeCollection Attributes => new AttributeCollectionImpl();
        public ConstantCollection Constants => new ConstantCollectionImpl();
        public FieldCollection Fields => new FieldCollectionImpl();
        public Class BaseClass => null;
        public Class ContainingClass => null;
        public string FullName => fullName;
        public TypeCollection GenericTypeArguments => new TypeCollectionImpl();
        public InterfaceCollection Interfaces => new InterfaceCollectionImpl();
        public bool IsEnum => false;
        public bool IsEnumerable => false;
        public bool IsGeneric => false;
        public bool IsNullable => false;
        public bool IsPrimitive => false;
        public MethodCollection Methods => new MethodCollectionImpl();
        public string Name => fullName;
        public string Namespace => null;
        public PropertyCollection Properties => new PropertyCollectionImpl();
        public ClassCollection NestedClasses => new ClassCollectionImpl();
        public EnumCollection NestedEnums => new EnumCollectionImpl();
        public InterfaceCollection NestedInterfaces => new InterfaceCollectionImpl();

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