using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;
using static Typewriter.CodeModel.Helpers;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class EnumValueImpl : EnumValue
    {
        private readonly IEnumValueMetadata metadata;

        private EnumValueImpl(IEnumValueMetadata metadata, Item parent)
        {
            this.metadata = metadata;
            this.Parent = parent;
        }

        public override Item Parent { get; }

        public override string name => CamelCase(metadata.Name);
        public override string Name => metadata.Name;
        public override string FullName => metadata.FullName;
        public override int Value => metadata.Value;

        private AttributeCollection attributes;
        public override AttributeCollection Attributes => attributes ?? (attributes = AttributeImpl.FromMetadata(metadata.Attributes, this));

        public override string ToString()
        {
            return Name;
        }

        public static EnumValueCollection FromMetadata(IEnumerable<IEnumValueMetadata> metadata, Item parent)
        {
            return new EnumValueCollectionImpl(metadata.Select(e => new EnumValueImpl(e, parent)));
        }
    }
}