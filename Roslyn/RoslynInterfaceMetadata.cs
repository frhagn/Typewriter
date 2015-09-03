using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynInterfaceMetadata : IInterfaceMetadata
    {
        private INamedTypeSymbol symbol;

        public RoslynInterfaceMetadata(INamedTypeSymbol symbol)
        {
            this.symbol = symbol;
        }

        public string Name => symbol.Name;
        public string FullName => symbol.ToDisplayString();
        public bool IsGeneric => symbol.TypeParameters.Any(); // Todo: IsGeneric???
        public string Namespace => symbol.GetNamespace();

        public IEnumerable<IAttributeMetadata> Attributes => RoslynAttributeMetadata.FromAttributeData(symbol.GetAttributes());
        public IClassMetadata ContainingClass => RoslynClassMetadata.FromNamedTypeSymbol(symbol.ContainingType);
        public IEnumerable<IEventMetadata> Events => RoslynEventMetadata.FromEventSymbols(symbol.GetMembers().OfType<IEventSymbol>());
        public IEnumerable<IInterfaceMetadata> Interfaces => RoslynInterfaceMetadata.FromNamedTypeSymbols(symbol.Interfaces);
        public IEnumerable<IMethodMetadata> Methods => RoslynMethodMetadata.FromMethodSymbols(symbol.GetMembers().OfType<IMethodSymbol>());
        public IEnumerable<IPropertyMetadata> Properties => RoslynPropertyMetadata.FromPropertySymbol(symbol.GetMembers().OfType<IPropertySymbol>());
        public IEnumerable<ITypeParameterMetadata> TypeParameters => RoslynTypeParameterMetadata.FromTypeParameterSymbols(symbol.TypeParameters);
        public IEnumerable<ITypeMetadata> TypeArguments => RoslynTypeMetadata.FromTypeSymbols(symbol.TypeArguments);

        public static IEnumerable<IInterfaceMetadata> FromNamedTypeSymbols(IEnumerable<INamedTypeSymbol> symbols)
        {
            return symbols.Where(s => s.DeclaredAccessibility == Accessibility.Public).Select(s => new RoslynInterfaceMetadata(s));
        }
    }
}