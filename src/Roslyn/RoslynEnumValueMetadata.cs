using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynEnumValueMetadata : IEnumValueMetadata
    {
        private readonly IFieldSymbol symbol;

        private RoslynEnumValueMetadata(IFieldSymbol symbol)
        {
            this.symbol = symbol;
        }

        public string Name => symbol.Name;
        public string FullName => symbol.ToDisplayString();
        public IEnumerable<IAttributeMetadata> Attributes => RoslynAttributeMetadata.FromAttributeData(symbol.GetAttributes());
        public int Value => (int)symbol.ConstantValue;

        internal static IEnumerable<IEnumValueMetadata> FromFieldSymbols(IEnumerable<IFieldSymbol> symbols)
        {
            return symbols.Select(s => new RoslynEnumValueMetadata(s));
        }
    }
}