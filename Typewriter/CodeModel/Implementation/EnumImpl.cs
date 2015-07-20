using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;
using static Typewriter.CodeModel.Helpers;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class EnumImpl : Enum
    {
        private readonly IEnumMetadata metadata;

        private EnumImpl(IEnumMetadata metadata, Item parent)
        {
            this.metadata = metadata;
            this.Parent = parent;
        }

        public override Item Parent { get; }

        public override string name => CamelCase(metadata.Name);
        public override string Name => metadata.Name;
        public override string FullName => metadata.FullName;
        public override string Namespace => metadata.Namespace;

        private bool? isFlags;
        public override bool IsFlags => isFlags ?? (isFlags = Attributes.Any(a => a.FullName == "System.FlagsAttribute")).Value;

        private AttributeCollection attributes;
        public override AttributeCollection Attributes => attributes ?? (attributes = AttributeImpl.FromMetadata(metadata.Attributes, this));

        private EnumValueCollection values;
        public override EnumValueCollection Values => values ?? (values = EnumValueImpl.FromMetadata(metadata.Values, this));

        private Class containingClass;
        public override Class ContainingClass => containingClass ?? (containingClass = ClassImpl.FromMetadata(metadata.ContainingClass, this));

        public override string ToString()
        {
            return Name;
        }

        public static EnumCollection FromMetadata(IEnumerable<IEnumMetadata> metadata, Item parent)
        {
            return new EnumCollectionImpl(metadata.Select(e => new EnumImpl(e, parent)));
        }
    }
}