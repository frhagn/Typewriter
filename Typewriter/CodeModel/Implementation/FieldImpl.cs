using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;
using static Typewriter.CodeModel.Helpers;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class FieldImpl : Field
    {
        private readonly IFieldMetadata metadata;

        private FieldImpl(IFieldMetadata metadata, Item parent)
        {
            this.metadata = metadata;
            this.Parent = parent;
        }

        public override Item Parent { get; }

        public override string name => CamelCase(metadata.Name);
        public override string Name => metadata.Name;
        public override string FullName => metadata.FullName;

        private AttributeCollection attributes;
        public override AttributeCollection Attributes => attributes ?? (attributes = AttributeImpl.FromMetadata(metadata.Attributes, this));

        private Type type;
        public override Type Type => type ?? (type = TypeImpl.FromMetadata(metadata.Type, this));

        public override string ToString()
        {
            return Name;
        }

        public static FieldCollection FromMetadata(IEnumerable<IFieldMetadata> metadata, Item parent)
        {
            return new FieldCollectionImpl(metadata.Select(f => new FieldImpl(f, parent)));
        }
    }
}