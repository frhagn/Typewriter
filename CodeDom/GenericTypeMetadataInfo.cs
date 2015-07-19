using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.CodeDom
{
    internal class GenericTypeMetadataInfo : ITypeMetadata
    {
        private readonly string fullName;
        private readonly CodeDomFileMetadata file;

        public GenericTypeMetadataInfo(string fullName, CodeDomFileMetadata file)
        {
            this.fullName = fullName;
            this.file = file;
        }

        public IEnumerable<IAttributeMetadata> Attributes => new IAttributeMetadata[0];
        public IClassMetadata BaseClass => null;
        public IClassMetadata ContainingClass => null;
        public IEnumerable<IConstantMetadata> Constants => new IConstantMetadata[0];
        public IEnumerable<IFieldMetadata> Fields => new IFieldMetadata[0];
        public IEnumerable<IInterfaceMetadata> Interfaces => new IInterfaceMetadata[0];
        public IEnumerable<IMethodMetadata> Methods => new IMethodMetadata[0];
        public IEnumerable<IPropertyMetadata> Properties => new IPropertyMetadata[0];
        public IEnumerable<ITypeMetadata> GenericTypeArguments => new ITypeMetadata[0];
        public IEnumerable<IClassMetadata> NestedClasses => new IClassMetadata[0];
        public IEnumerable<IEnumMetadata> NestedEnums => new IEnumMetadata[0];
        public IEnumerable<IInterfaceMetadata> NestedInterfaces => new IInterfaceMetadata[0];
        public string FullName => fullName;
        public bool IsEnum => false;
        public bool IsEnumerable => false;
        public bool IsGeneric => false;
        public bool IsNullable => false;
        public bool IsPrimitive => false;
        public string Name => fullName;
        public string Namespace => null;
        
        internal static IEnumerable<ITypeMetadata> FromFullName(string fullName, CodeDomFileMetadata file)
        {
            return ExtractGenericTypeNames(fullName).Select(n => new GenericTypeMetadataInfo(n, file));
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