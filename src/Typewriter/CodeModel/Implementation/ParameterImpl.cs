using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;
using static Typewriter.CodeModel.Helpers;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class ParameterImpl : Parameter
    {
        private readonly IParameterMetadata _metadata;

        private ParameterImpl(IParameterMetadata metadata, Item parent)
        {
            _metadata = metadata;
            Parent = parent;
        }

        public override Item Parent { get; }

        public override string name => CamelCase(_metadata.Name.TrimStart('@'));
        public override string Name => _metadata.Name.TrimStart('@');
        public override string FullName => _metadata.FullName;
        public override bool HasDefaultValue => _metadata.HasDefaultValue;
        public override string DefaultValue => _metadata.DefaultValue;

        private AttributeCollection _attributes;
        public override AttributeCollection Attributes => _attributes ?? (_attributes = AttributeImpl.FromMetadata(_metadata.Attributes, this));

        private Type _type;
        public override Type Type => _type ?? (_type = TypeImpl.FromMetadata(_metadata.Type, this));

        public override string ToString()
        {
            return Name;
        }

        public static ParameterCollection FromMetadata(IEnumerable<IParameterMetadata> metadata, Item parent)
        {
            return new ParameterCollectionImpl(metadata.Select(p => new ParameterImpl(p, parent)));
        }
    }
}