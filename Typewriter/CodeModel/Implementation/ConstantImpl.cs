using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class ConstantImpl : Constant
    {
        private readonly IConstantMetadata metadata;
        private readonly Item parent;

        private ConstantImpl(IConstantMetadata metadata, Item parent)
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

        public static ConstantCollection FromMetadata(IEnumerable<IConstantMetadata> metadata, Item parent)
        {
            return new ConstantCollectionImpl(metadata.Select(c => new ConstantImpl(c, parent)));
        }
    }
}