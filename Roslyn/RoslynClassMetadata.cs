using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynClassMetadata : IClassMetadata
    {
        private readonly INamedTypeSymbol symbol;

        private RoslynClassMetadata(INamedTypeSymbol symbol)
        {
            this.symbol = symbol;
        }

        public string Name => symbol.Name;
        public string FullName => symbol.ToDisplayString();
        public bool IsGeneric => symbol.TypeParameters.Any(); // Todo: IsGeneric???
        public string Namespace => symbol.GetNamespace();

        public IEnumerable<IAttributeMetadata> Attributes => RoslynAttributeMetadata.FromAttributeData(symbol.GetAttributes());
        public IClassMetadata BaseClass => RoslynClassMetadata.FromNamedTypeSymbol(symbol.BaseType);
        public IClassMetadata ContainingClass => RoslynClassMetadata.FromNamedTypeSymbol(symbol.ContainingType);
        public IEnumerable<IConstantMetadata> Constants => RoslynConstantMetadata.FromFieldSymbols(symbol.GetMembers().OfType<IFieldSymbol>());
        public IEnumerable<IFieldMetadata> Fields => RoslynFieldMetadata.FromFieldSymbols(symbol.GetMembers().OfType<IFieldSymbol>());
        public IEnumerable<IInterfaceMetadata> Interfaces => RoslynInterfaceMetadata.FromNamedTypeSymbols(symbol.Interfaces);
        public IEnumerable<IMethodMetadata> Methods => RoslynMethodMetadata.FromMethodSymbols(symbol.GetMembers().OfType<IMethodSymbol>());
        public IEnumerable<IPropertyMetadata> Properties => RoslynPropertyMetadata.FromPropertySymbol(symbol.GetMembers().OfType<IPropertySymbol>());
        public IEnumerable<ITypeParameterMetadata> TypeParameters => RoslynTypeParameterMetadata.FromTypeParameterSymbols(symbol.TypeParameters);
        public IEnumerable<ITypeMetadata> TypeArguments => RoslynTypeMetadata.FromTypeSymbols(((INamedTypeSymbol)symbol).TypeArguments);
        public IEnumerable<IClassMetadata> NestedClasses => RoslynClassMetadata.FromNamedTypeSymbols(symbol.GetMembers().OfType<INamedTypeSymbol>().Where(s => s.TypeKind == TypeKind.Class));
        public IEnumerable<IEnumMetadata> NestedEnums => RoslynEnumMetadata.FromNamedTypeSymbols(symbol.GetMembers().OfType<INamedTypeSymbol>().Where(s => s.TypeKind == TypeKind.Enum));
        public IEnumerable<IInterfaceMetadata> NestedInterfaces => RoslynInterfaceMetadata.FromNamedTypeSymbols(symbol.GetMembers().OfType<INamedTypeSymbol>().Where(s => s.TypeKind == TypeKind.Interface));

        internal static IClassMetadata FromNamedTypeSymbol(INamedTypeSymbol symbol)
        {
            if (symbol?.DeclaredAccessibility != Accessibility.Public || symbol.ToDisplayString() == "object") return null;

            return new RoslynClassMetadata(symbol);
        }

        internal static IEnumerable<IClassMetadata> FromNamedTypeSymbols(IEnumerable<INamedTypeSymbol> symbols)
        {
            return symbols.Where(s => s.DeclaredAccessibility == Accessibility.Public && s.ToDisplayString() != "object").Select(s => new RoslynClassMetadata(s));
        }
    }
}
