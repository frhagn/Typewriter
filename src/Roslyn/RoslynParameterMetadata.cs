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
        public bool HasDefaultValue => symbol.HasExplicitDefaultValue;
        public string DefaultValue => GetDefaultValue();
        public IEnumerable<IAttributeMetadata> Attributes => RoslynAttributeMetadata.FromAttributeData(symbol.GetAttributes());
        public ITypeMetadata Type => RoslynTypeMetadata.FromTypeSymbol(symbol.Type);

        private string GetDefaultValue()
        {
            if (symbol.HasExplicitDefaultValue == false)
                return null;

            if (symbol.ExplicitDefaultValue == null)
                return "null";

            var stringValue = symbol.ExplicitDefaultValue as string;
            if (stringValue != null)
                return $"\"{stringValue.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"";

            if(symbol.ExplicitDefaultValue is bool)
                return (bool)symbol.ExplicitDefaultValue ? "true" : "false";

            return symbol.ExplicitDefaultValue.ToString();
        }

        public static IEnumerable<IParameterMetadata> FromParameterSymbols(IEnumerable<IParameterSymbol> symbols)
        {
            return symbols.Select(s => new RoslynParameterMetadata(s));
        }
    }
}