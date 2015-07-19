using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class AttributeImpl : Attribute
    {
        private readonly IAttributeMetadata metadata;
        private readonly Item parent;

        private AttributeImpl(IAttributeMetadata metadata, Item parent)
        {
            this.metadata = metadata;
            this.parent = parent;
        }

        public Item Parent => parent;
        public AttributeCollection Attributes { get; } // Todo: Remove from interface
        public string Name => metadata.Name;
        public string FullName => metadata.FullName;
        public string Value => metadata.Value;

        public static AttributeCollection FromMetadata(IEnumerable<IAttributeMetadata> metadata, Item parent)
        {
            return new AttributeCollectionImpl(metadata.Select(a => new AttributeImpl(a, parent)));
        }
    }
}