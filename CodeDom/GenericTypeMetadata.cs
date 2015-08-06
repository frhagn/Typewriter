using System.Collections.Generic;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.CodeDom
{
    internal class GenericTypeMetadata : ITypeMetadata
    {
        private readonly string fullName;

        public GenericTypeMetadata(string fullName)
        {
            this.fullName = fullName;
        }

        public IEnumerable<IAttributeMetadata> Attributes => new IAttributeMetadata[0];
        public IEnumerable<ITypeMetadata> TypeArguments => new ITypeMetadata[0];
        public string FullName => fullName;
        public bool IsEnum => false;
        public bool IsEnumerable => false;
        public bool IsGeneric => false;
        public bool IsNullable => false;
        public string Name => fullName;
        public string Namespace => null;
    }
}