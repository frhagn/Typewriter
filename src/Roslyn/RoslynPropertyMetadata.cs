using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynPropertyMetadata : IPropertyMetadata
    {
        private readonly IPropertySymbol symbol;

        private RoslynPropertyMetadata(IPropertySymbol symbol)
        {
            this.symbol = symbol;
        }

        public string DocComment => symbol.GetDocumentationCommentXml();
        public string Name => symbol.Name;
        public string FullName => symbol.ToDisplayString();
        public IEnumerable<IAttributeMetadata> Attributes => RoslynAttributeMetadata.FromAttributeData(symbol.GetAttributes());
        public ITypeMetadata Type => RoslynTypeMetadata.FromTypeSymbol(symbol.Type);
        public bool IsAbstract => symbol.IsAbstract;
        public bool HasGetter => symbol.GetMethod != null && symbol.GetMethod.DeclaredAccessibility == Accessibility.Public;
        public bool HasSetter => symbol.SetMethod != null && symbol.SetMethod.DeclaredAccessibility == Accessibility.Public;
        
        public static IEnumerable<IPropertyMetadata> FromPropertySymbol(IEnumerable<IPropertySymbol> symbols)
        {
            return symbols.Where(p => p.DeclaredAccessibility == Accessibility.Public && p.IsStatic == false).Select(p => new RoslynPropertyMetadata(p));
        }
    }
}