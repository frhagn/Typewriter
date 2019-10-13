using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.CodeAnalysis;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynEnumValueMetadata : IEnumValueMetadata
    {
        private static readonly Int64Converter _converter = new Int64Converter();

        private readonly IFieldSymbol symbol;

        private RoslynEnumValueMetadata(IFieldSymbol symbol)
        {
            this.symbol = symbol;
        }

        public string DocComment => symbol.GetDocumentationCommentXml();
        public string Name => symbol.Name;
        public string FullName => symbol.ToDisplayString();
        public IEnumerable<IAttributeMetadata> Attributes => RoslynAttributeMetadata.FromAttributeData(symbol.GetAttributes());
        public long Value => (long?)_converter.ConvertFromString(symbol.ConstantValue.ToString().Trim('\'')) ?? -1;

        internal static IEnumerable<IEnumValueMetadata> FromFieldSymbols(IEnumerable<IFieldSymbol> symbols)
        {
            return symbols.Select(s => new RoslynEnumValueMetadata(s));
        }
    }
}