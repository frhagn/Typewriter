using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class EnumValueImpl : EnumValue
    {
        private readonly IEnumValueMetadata metadata;
        private readonly Item parent;

        private EnumValueImpl(IEnumValueMetadata metadata, Item parent)
        {
            this.metadata = metadata;
            this.parent = parent;
        }

        public Item Parent => parent;
        public string Name => metadata.Name;
        public string FullName => metadata.FullName;
        public int Value => metadata.Value;

        private AttributeCollection attributes;
        public AttributeCollection Attributes => attributes ?? (attributes = AttributeImpl.FromMetadata(metadata.Attributes, this));

        public static EnumValueCollection FromMetadata(IEnumerable<IEnumValueMetadata> metadata, Item parent)
        {
            return new EnumValueCollectionImpl(metadata.Select(e => new EnumValueImpl(e, parent)));
        }
    }
}