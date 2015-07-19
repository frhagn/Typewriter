using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class PropertyImpl : Property
    {
        private readonly IPropertyMetadata metadata;
        private readonly Item parent;

        private PropertyImpl(IPropertyMetadata metadata, Item parent)
        {
            this.metadata = metadata;
            this.parent = parent;
        }

        public Item Parent => parent;
        public string Name => metadata.Name;
        public string FullName => metadata.FullName;
        public bool HasGetter => metadata.HasGetter;
        public bool HasSetter => metadata.HasSetter;

        private AttributeCollection attributes;
        public AttributeCollection Attributes => attributes ?? (attributes = AttributeImpl.FromMetadata(metadata.Attributes, this));

        private Type type;
        public Type Type => type ?? (type = TypeImpl.FromMetadata(metadata.Type, this));

        public static PropertyCollection FromMetadata(IEnumerable<IPropertyMetadata> metadata, Item parent)
        {
            return new PropertyCollectionImpl(metadata.Select(p => new PropertyImpl(p, parent)));
        }
    }
}