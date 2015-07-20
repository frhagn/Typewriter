using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;
using static Typewriter.CodeModel.Helpers;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class AttributeImpl : Attribute
    {
        private readonly IAttributeMetadata metadata;

        private AttributeImpl(IAttributeMetadata metadata, Item parent)
        {
            this.metadata = metadata;
            this.Parent = parent;
        }

        public override Item Parent { get; }

        public override string name => CamelCase(metadata.Name);
        public override string Name => metadata.Name;
        public override string FullName => metadata.FullName;

        public override string Value => metadata.Value;

        public override string ToString()
        {
            return Name;
        }

        public static AttributeCollection FromMetadata(IEnumerable<IAttributeMetadata> metadata, Item parent)
        {
            return new AttributeCollectionImpl(metadata.Select(a => new AttributeImpl(a, parent)));
        }
    }
}