using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynAttributeMetadata : IAttributeMetadata
    {
        private readonly INamedTypeSymbol symbol;
        private readonly string name;
        private readonly string value;

        private RoslynAttributeMetadata(AttributeData a)
        {
            var declaration = a.ToString();
            var index = declaration.IndexOf("(", StringComparison.Ordinal);

            this.symbol = a.AttributeClass;
            this.name = symbol.Name;

            if (index > -1)
            {
                this.value = declaration.Substring(index + 1, declaration.Length - index - 2);

                // Trim {} from params
                if (this.value.EndsWith("\"}"))
                {
                    this.value = this.value.Remove(this.value.LastIndexOf("{\"", StringComparison.Ordinal), 1);
                    this.value = this.value.TrimEnd('}');
                }
                else if (this.value.EndsWith("}"))
                {
                    this.value = this.value.Remove(this.value.LastIndexOf("{", StringComparison.Ordinal), 1);
                    this.value = this.value.TrimEnd('}');
                }
            }

            if (name.EndsWith("Attribute"))
                name = name.Substring(0, name.Length - 9);

            this.Arguments = a.ConstructorArguments.Concat(a.NamedArguments.Select(p=>p.Value)).Select(p => new RoslynAttrubuteArgumentMetadata(p));
        }

        public string DocComment => symbol.GetDocumentationCommentXml();
        public string Name => name;
        public string FullName => symbol.ToDisplayString();
        public string Value => value;
        public IEnumerable<IAttributeArgumentMetadata> Arguments { get; private set; }

        public static IEnumerable<IAttributeMetadata> FromAttributeData(IEnumerable<AttributeData> attributes)
        {
            return attributes.Select(a => new RoslynAttributeMetadata(a));
        }
    }
}