using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynTypeMetadata : ITypeMetadata
    {
        private readonly ITypeSymbol symbol;
        private readonly bool isNullable;

        public RoslynTypeMetadata(ITypeSymbol symbol, bool isNullable)
        {
            this.symbol = symbol;
            this.isNullable = isNullable;
        }

        public string Name => symbol.Name + (IsNullable? "?" : string.Empty);
        public string FullName => symbol.GetFullName() + (IsNullable? "?" : string.Empty);
        public bool IsGeneric => (symbol as INamedTypeSymbol)?.TypeParameters.Any() ?? false;
        public string Namespace => symbol.GetNamespace();

        public IEnumerable<IAttributeMetadata> Attributes => RoslynAttributeMetadata.FromAttributeData(symbol.GetAttributes());
        public IEnumerable<ITypeMetadata> TypeArguments => (symbol is INamedTypeSymbol) ? FromTypeSymbols(((INamedTypeSymbol)symbol).TypeArguments) : new ITypeMetadata[0];
        
        public bool IsEnum => symbol.TypeKind == TypeKind.Enum;
        public bool IsEnumerable => symbol.ToDisplayString() != "string" && (
            symbol.TypeKind == TypeKind.Array ||
            symbol.ToDisplayString() == "System.Collections.IEnumerable" ||
            symbol.AllInterfaces.Any(i => i.ToDisplayString() == "System.Collections.IEnumerable"));
        public bool IsNullable => isNullable;

        public static ITypeMetadata FromTypeSymbol(ITypeSymbol symbol)
        {
            if (symbol.Name == "Nullable" && symbol.ContainingNamespace.Name == "System")
            {
                var type = symbol as INamedTypeSymbol;

                if (type != null)
                    return new RoslynTypeMetadata(type.TypeArguments.First(), true);
            }

            return new RoslynTypeMetadata(symbol, false);
        }

        public static IEnumerable<ITypeMetadata> FromTypeSymbols(IEnumerable<ITypeSymbol> symbols)
        {
            return symbols.Select(FromTypeSymbol);
        }
    }
}