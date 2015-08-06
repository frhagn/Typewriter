using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynParameterMetadata : IParameterMetadata
    {
        private readonly IParameterSymbol symbol;

        private RoslynParameterMetadata(IParameterSymbol symbol)
        {
            this.symbol = symbol;
        }

        public string Name => symbol.Name;
        public string FullName => symbol.ToDisplayString();
        public IEnumerable<IAttributeMetadata> Attributes => RoslynAttributeMetadata.FromAttributeData(symbol.GetAttributes());
        public ITypeMetadata Type => RoslynTypeMetadata.FromTypeSymbol(symbol.Type);

        public static IEnumerable<IParameterMetadata> FromParameterSymbols(IEnumerable<IParameterSymbol> symbols)
        {
            return symbols.Select(s => new RoslynParameterMetadata(s));
        }
    }
}