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
        public IClassMetadata BaseClass => RoslynClassMetadata.FromNamedTypeSymbol(symbol.BaseType);
        public IClassMetadata ContainingClass => RoslynClassMetadata.FromNamedTypeSymbol(symbol.ContainingType);

        public IEnumerable<IConstantMetadata> Constants => RoslynConstantMetadata.FromFieldSymbols(symbol.GetMembers().OfType<IFieldSymbol>());
        public IEnumerable<IFieldMetadata> Fields => RoslynFieldMetadata.FromFieldSymbols(symbol.GetMembers().OfType<IFieldSymbol>());
        public IEnumerable<IInterfaceMetadata> Interfaces => RoslynInterfaceMetadata.FromNamedTypeSymbols(symbol.Interfaces);
        public IEnumerable<IMethodMetadata> Methods => RoslynMethodMetadata.FromMethodSymbols(symbol.GetMembers().OfType<IMethodSymbol>());
        public IEnumerable<IPropertyMetadata> Properties => RoslynPropertyMetadata.FromPropertySymbol(symbol.GetMembers().OfType<IPropertySymbol>());
        public IEnumerable<IClassMetadata> NestedClasses => RoslynClassMetadata.FromNamedTypeSymbols(symbol.GetMembers().OfType<INamedTypeSymbol>().Where(s => s.TypeKind == TypeKind.Class));
        public IEnumerable<IEnumMetadata> NestedEnums => RoslynEnumMetadata.FromNamedTypeSymbols(symbol.GetMembers().OfType<INamedTypeSymbol>().Where(s => s.TypeKind == TypeKind.Enum));
        public IEnumerable<IInterfaceMetadata> NestedInterfaces => RoslynInterfaceMetadata.FromNamedTypeSymbols(symbol.GetMembers().OfType<INamedTypeSymbol>().Where(s => s.TypeKind == TypeKind.Interface));


        public IEnumerable<ITypeMetadata> TypeArguments
        {
            get
            {
                var namedTypeSymbol = symbol as INamedTypeSymbol;
                if (namedTypeSymbol != null)
                    return FromTypeSymbols(namedTypeSymbol.TypeArguments);

                var arrayTypeSymbol = symbol as IArrayTypeSymbol;
                if (arrayTypeSymbol != null)
                    return FromTypeSymbols(new [] { arrayTypeSymbol.ElementType});

                return new ITypeMetadata[0];
            }
        }

        public IEnumerable<ITypeParameterMetadata> TypeParameters
        {
            get
            {
                var namedTypeSymbol = symbol as INamedTypeSymbol;
                if (namedTypeSymbol != null)
                    return RoslynTypeParameterMetadata.FromTypeParameterSymbols(namedTypeSymbol.TypeParameters);

                return new ITypeParameterMetadata[0];
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
        public IClassMetadata BaseClass => null;
        public IClassMetadata ContainingClass => null;
        public IEnumerable<IConstantMetadata> Constants => new IConstantMetadata[0];
        public IEnumerable<IFieldMetadata> Fields => new IFieldMetadata[0];
        public IEnumerable<IInterfaceMetadata> Interfaces => new IInterfaceMetadata[0];
        public IEnumerable<IMethodMetadata> Methods => new IMethodMetadata[0];
        public IEnumerable<IPropertyMetadata> Properties => new IPropertyMetadata[0];
        public IEnumerable<IClassMetadata> NestedClasses => new IClassMetadata[0];
        public IEnumerable<IEnumMetadata> NestedEnums => new IEnumMetadata[0];
        public IEnumerable<IInterfaceMetadata> NestedInterfaces => new IInterfaceMetadata[0];
        public IEnumerable<ITypeMetadata> TypeArguments => new ITypeMetadata[0];
        public IEnumerable<ITypeParameterMetadata> TypeParameters => new ITypeParameterMetadata[0];
    }
}