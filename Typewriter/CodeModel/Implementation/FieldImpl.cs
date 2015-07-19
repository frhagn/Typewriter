using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class FieldImpl : Field
    {
        private readonly IFieldMetadata metadata;
        private readonly Item parent;

        private FieldImpl(IFieldMetadata metadata, Item parent)
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

        public static FieldCollection FromMetadata(IEnumerable<IFieldMetadata> metadata, Item parent)
        {
            return new FieldCollectionImpl(metadata.Select(f => new FieldImpl(f, parent)));
        }
    }
}