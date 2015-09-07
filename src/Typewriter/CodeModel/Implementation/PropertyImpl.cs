using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;
using static Typewriter.CodeModel.Helpers;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class PropertyImpl : Property
    {
        private readonly IPropertyMetadata metadata;

        private PropertyImpl(IPropertyMetadata metadata, Item parent)
        {
            this.metadata = metadata;
            this.Parent = parent;
        }

        public override Item Parent { get; }

        public override string name => CamelCase(metadata.Name);
        public override string Name => metadata.Name;
        public override string FullName => metadata.FullName;
        public override bool HasGetter => metadata.HasGetter;
        public override bool HasSetter => metadata.HasSetter;

        private AttributeCollection attributes;
        public override AttributeCollection Attributes => attributes ?? (attributes = AttributeImpl.FromMetadata(metadata.Attributes, this));

        private Type type;
        public override Type Type => type ?? (type = TypeImpl.FromMetadata(metadata.Type, this));

        public override string ToString()
        {
            return Name;
        }

        public static PropertyCollection FromMetadata(IEnumerable<IPropertyMetadata> metadata, Item parent)
        {
            return new PropertyCollectionImpl(metadata.Select(p => new PropertyImpl(p, parent)));
        }
    }
}