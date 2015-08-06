using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.CodeModel.Implementation
{
    public class TypeParameterImpl : TypeParameter
    {
        private readonly ITypeParameterMetadata metadata;

        public TypeParameterImpl(ITypeParameterMetadata metadata, Item parent)
        {
            this.metadata = metadata;
            this.Parent = parent;
        }

        public override Item Parent { get; }
        public override string name => Helpers.CamelCase(metadata.Name);
        public override string Name => metadata.Name;

        public static TypeParameterCollection FromMetadata(IEnumerable<ITypeParameterMetadata> metadata, Item parent)
        {
            return new TypeParameterCollectionImpl(metadata.Select(t => new TypeParameterImpl(t, parent)));
        }
    }
}