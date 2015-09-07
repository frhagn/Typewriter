using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynConstantMetadata : RoslynFieldMetadata, IConstantMetadata
    {
        private RoslynConstantMetadata(IFieldSymbol symbol) : base(symbol)
        {
        }

        public new static IEnumerable<IConstantMetadata> FromFieldSymbols(IEnumerable<IFieldSymbol> symbols)
        {
            return symbols.Where(s => s.DeclaredAccessibility == Accessibility.Public && s.IsConst).Select(s => new RoslynConstantMetadata(s));
        }
    }
}