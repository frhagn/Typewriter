using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynTypeMetadata : ITypeMetadata
    {
        private readonly ITypeSymbol symbol;
        private readonly bool isNullable;
        private readonly bool isTask;

        public RoslynTypeMetadata(ITypeSymbol symbol, bool isNullable, bool isTask)
        {
            this.symbol = symbol;
            this.isNullable = isNullable;
            this.isTask = isTask;
        }

        public string Name => symbol.Name + (IsNullable? "?" : string.Empty);
        public string FullName => symbol.GetFullName() + (IsNullable? "?" : string.Empty);
        public bool IsGeneric => (symbol as INamedTypeSymbol)?.TypeParameters.Any() ?? false;
        public string Namespace => symbol.GetNamespace();

        public IEnumerable<IAttributeMetadata> Attributes => RoslynAttributeMetadata.FromAttributeData(symbol.GetAttributes());
        public IEnumerable<ITypeMetadata> TypeArguments
        {
            get
            {
                if (symbol is INamedTypeSymbol)
                {
                    return FromTypeSymbols(((INamedTypeSymbol) symbol).TypeArguments);
                }

                if (symbol is IArrayTypeSymbol)
                {
                    return FromTypeSymbols(new [] { ((IArrayTypeSymbol) symbol).ElementType});
                }

                return new ITypeMetadata[0];
            }
        }

        public bool IsEnum => symbol.TypeKind == TypeKind.Enum;
        public bool IsEnumerable => symbol.ToDisplayString() != "string" && (
            symbol.TypeKind == TypeKind.Array ||
            symbol.ToDisplayString() == "System.Collections.IEnumerable" ||
            symbol.AllInterfaces.Any(i => i.ToDisplayString() == "System.Collections.IEnumerable"));
        public bool IsNullable => isNullable;
        public bool IsTask => isTask;

        public static ITypeMetadata FromTypeSymbol(ITypeSymbol symbol)
        {
            if (symbol.Name == "Nullable" && symbol.ContainingNamespace.Name == "System")
            {
                var type = symbol as INamedTypeSymbol;
                var argument = type?.TypeArguments.FirstOrDefault();

                if (argument != null)
                    return new RoslynTypeMetadata(argument, true, false);
            }
            else if (symbol.Name == "Task" && symbol.ContainingNamespace.GetFullName() == "System.Threading.Tasks")
            {
                var type = symbol as INamedTypeSymbol;
                var argument = type?.TypeArguments.FirstOrDefault();

                if (argument != null)
                {
                    if (argument.Name == "Nullable" && argument.ContainingNamespace.Name == "System")
                    {
                        type = argument as INamedTypeSymbol;
                        var innerArgument = type?.TypeArguments.FirstOrDefault();

                        if (innerArgument != null)
                            return new RoslynTypeMetadata(innerArgument, true, true);
                    }

                    return new RoslynTypeMetadata(argument, false, true);
                }

                return new RoslynVoidTaskMetadata();
            }

            return new RoslynTypeMetadata(symbol, false, false);
        }

        public static IEnumerable<ITypeMetadata> FromTypeSymbols(IEnumerable<ITypeSymbol> symbols)
        {
            return symbols.Select(FromTypeSymbol);
        }
    }

    public class RoslynVoidTaskMetadata : ITypeMetadata
    {
        public string Name => "Void";
        public string FullName => "System.Void";
        public bool IsEnum => false;
        public bool IsEnumerable => false;
        public bool IsGeneric => false;
        public bool IsNullable => false;
        public bool IsTask => true;
        public string Namespace => "System";
        public IEnumerable<IAttributeMetadata> Attributes => new IAttributeMetadata[0];
        public IEnumerable<ITypeMetadata> TypeArguments => new ITypeMetadata[0];
    }
}