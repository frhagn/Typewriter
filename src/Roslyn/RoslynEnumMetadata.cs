using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynEnumMetadata : IEnumMetadata
    {
        private INamedTypeSymbol symbol;

        public RoslynEnumMetadata(INamedTypeSymbol symbol)
        {
            this.symbol = symbol;
        }

        public string Name => symbol.Name;
        public string FullName => symbol.ToDisplayString();
        public string Namespace => symbol.GetNamespace();

        public IEnumerable<IAttributeMetadata> Attributes => RoslynAttributeMetadata.FromAttributeData(symbol.GetAttributes());
        public IClassMetadata ContainingClass => RoslynClassMetadata.FromNamedTypeSymbol(symbol.ContainingType);
        public IEnumerable<IEnumValueMetadata> Values => RoslynEnumValueMetadata.FromFieldSymbols(symbol.GetMembers().OfType<IFieldSymbol>());
        
        internal static IEnumerable<IEnumMetadata> FromNamedTypeSymbols(IEnumerable<INamedTypeSymbol> symbols)
        {
            return symbols.Where(s => s.DeclaredAccessibility == Accessibility.Public).Select(s => new RoslynEnumMetadata(s));
        }
    }
}