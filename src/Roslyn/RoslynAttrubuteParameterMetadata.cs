using Microsoft.CodeAnalysis;
using System.Linq;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynAttrubuteArgumentMetadata : IAttributeArgumentMetadata
    {
        private TypedConstant typeConstant;

        public RoslynAttrubuteArgumentMetadata(TypedConstant typeConstant)
        {
            this.typeConstant = typeConstant;
        }

        public ITypeMetadata Type => RoslynTypeMetadata.FromTypeSymbol(typeConstant.Type);

        public ITypeMetadata TypeValue => typeConstant.Kind == TypedConstantKind.Type ? RoslynTypeMetadata.FromTypeSymbol((INamedTypeSymbol)typeConstant.Value) : null;
        public object Value => typeConstant.Kind == TypedConstantKind.Array ? typeConstant.Values.Select(prop => prop.Value).ToArray() : typeConstant.Value;
    }
}