using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;
using static Typewriter.CodeModel.Helpers;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class EnumValueImpl : EnumValue
    {
        private readonly IEnumValueMetadata _metadata;

        private EnumValueImpl(IEnumValueMetadata metadata, Item parent)
        {
            _metadata = metadata;
            Parent = parent;
        }

        public override Item Parent { get; }

        public override string name => CamelCase(_metadata.Name.TrimStart('@'));
        public override string Name => _metadata.Name.TrimStart('@');
        public override string FullName => _metadata.FullName;
        public override long Value => _metadata.Value;

        private AttributeCollection _attributes;
        public override AttributeCollection Attributes => _attributes ?? (_attributes = AttributeImpl.FromMetadata(_metadata.Attributes, this));

        private DocComment _docComment;
        public override DocComment DocComment => _docComment ?? (_docComment = DocCommentImpl.FromXml(_metadata.DocComment, this));

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