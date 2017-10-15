using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynEnumMetadata : IEnumMetadata
    {
        private readonly INamedTypeSymbol _symbol;

        public RoslynEnumMetadata(INamedTypeSymbol symbol)
        {
            _symbol = symbol;
        }

        public string DocComment => _symbol.GetDocumentationCommentXml();
        public string Name => _symbol.Name;
        public string FullName => _symbol.ToDisplayString();
        public string Namespace => _symbol.GetNamespace();

        public ITypeMetadata Type => RoslynTypeMetadata.FromTypeSymbol(_symbol);

        public IEnumerable<IAttributeMetadata> Attributes => RoslynAttributeMetadata.FromAttributeData(_symbol.GetAttributes());
        public IClassMetadata ContainingClass => RoslynClassMetadata.FromNamedTypeSymbol(_symbol.ContainingType);
        public IEnumerable<IEnumValueMetadata> Values => RoslynEnumValueMetadata.FromFieldSymbols(_symbol.GetMembers().OfType<IFieldSymbol>());
        
        internal static IEnumerable<IEnumMetadata> FromNamedTypeSymbols(IEnumerable<INamedTypeSymbol> symbols)
        {
            return symbols.Where(s => s.DeclaredAccessibility == Accessibility.Public).Select(s => new RoslynEnumMetadata(s));
        }
    }
}