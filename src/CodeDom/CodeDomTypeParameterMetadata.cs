using System.Collections.Generic;
using System.Linq;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.CodeDom
{
    internal class CodeDomTypeParameterMetadata : ITypeParameterMetadata
    {
        private readonly string fullName;

        public CodeDomTypeParameterMetadata(string fullName)
        {
            this.fullName = fullName;
        }

        public string Name => fullName;

        internal static IEnumerable<ITypeParameterMetadata> FromFullName(string fullName)
        {
            return LazyCodeDomTypeMetadata.ExtractGenericTypeNames(fullName).Select(n => new CodeDomTypeParameterMetadata(n.Trim()));
        }
    }
}