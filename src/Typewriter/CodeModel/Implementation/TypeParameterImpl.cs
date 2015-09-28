using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;
using static Typewriter.CodeModel.Helpers;

namespace Typewriter.CodeModel.Implementation
{
    public class TypeParameterImpl : TypeParameter
    {
        private readonly ITypeParameterMetadata _metadata;

        public TypeParameterImpl(ITypeParameterMetadata metadata, Item parent)
        {
            _metadata = metadata;
            Parent = parent;
        }

        public override Item Parent { get; }
        public override string name => CamelCase(_metadata.Name.TrimStart('@'));
        public override string Name => _metadata.Name.TrimStart('@');

        public static TypeParameterCollection FromMetadata(IEnumerable<ITypeParameterMetadata> metadata, Item parent)
        {
            return new TypeParameterCollectionImpl(metadata.Select(t => new TypeParameterImpl(t, parent)));
        }
    }
}