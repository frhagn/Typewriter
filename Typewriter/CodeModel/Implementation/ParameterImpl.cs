using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class ParameterImpl : Parameter
    {
        private readonly IParameterMetadata metadata;
        private readonly Item parent;

        private ParameterImpl(IParameterMetadata metadata, Item parent)
        {
            this.metadata = metadata;
            this.parent = parent;
        }

        public Item Parent => parent;
        public string Name => metadata.Name;
        public string FullName => metadata.FullName;

        private AttributeCollection attributes;
        public AttributeCollection Attributes => attributes ?? (attributes = AttributeImpl.FromMetadata(metadata.Attributes, this));

        private Type type;
        public Type Type => type ?? (type = TypeImpl.FromMetadata(metadata.Type, this));

        public static ParameterCollection FromMetadata(IEnumerable<IParameterMetadata> metadata, Item parent)
        {
            return new ParameterCollectionImpl(metadata.Select(p => new ParameterImpl(p, parent)));
        }
    }
}