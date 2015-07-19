using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class EnumImpl : Enum
    {
        private readonly IEnumMetadata metadata;
        private readonly Item parent;

        private EnumImpl(IEnumMetadata metadata, Item parent)
        {
            this.metadata = metadata;
            this.parent = parent;
        }

        public Item Parent => parent;
        public string Name => metadata.Name;
        public string FullName => metadata.FullName;
        public string Namespace => metadata.Namespace;

        private bool? isFlags;
        public bool IsFlags => isFlags ?? (isFlags = Attributes.Any(a => a.FullName == "System.FlagsAttribute")).Value;

        private AttributeCollection attributes;
        public AttributeCollection Attributes => attributes ?? (attributes = AttributeImpl.FromMetadata(metadata.Attributes, this));

        private EnumValueCollection values;
        public EnumValueCollection Values => values ?? (values = EnumValueImpl.FromMetadata(metadata.Values, this));

        private Class containingClass;
        public Class ContainingClass => containingClass ?? (containingClass = ClassImpl.FromMetadata(metadata.ContainingClass, this));

        public static EnumCollection FromMetadata(IEnumerable<IEnumMetadata> metadata, Item parent)
        {
            return new EnumCollectionImpl(metadata.Select(e => new EnumImpl(e, parent)));
        }
    }
}