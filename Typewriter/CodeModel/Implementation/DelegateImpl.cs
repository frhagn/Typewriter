using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;
using static Typewriter.CodeModel.Helpers;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class DelegateImpl : Delegate
    {
        private readonly IDelegateMetadata metadata;

        private DelegateImpl(IDelegateMetadata metadata, Item parent)
        {
            this.metadata = metadata;
            this.Parent = parent;
        }

        public override Item Parent { get; }

        public override string name => CamelCase(metadata.Name);
        public override string Name => metadata.Name;
        public override string FullName => metadata.FullName;
        public override bool IsGeneric => metadata.IsGeneric;

        private AttributeCollection attributes;
        public override AttributeCollection Attributes => attributes ?? (attributes = AttributeImpl.FromMetadata(metadata.Attributes, this));

        private TypeParameterCollection typeParameters;
        public override TypeParameterCollection TypeParameters => typeParameters ?? (typeParameters = TypeParameterImpl.FromMetadata(metadata.TypeParameters, this));

        private ParameterCollection parameters;
        public override ParameterCollection Parameters => parameters ?? (parameters = ParameterImpl.FromMetadata(metadata.Parameters, this));

        private Type type;
        public override Type Type => type ?? (type = TypeImpl.FromMetadata(metadata.Type, this));

        public override string ToString()
        {
            return Name;
        }

        public static DelegateCollection FromMetadata(IEnumerable<IDelegateMetadata> metadata, Item parent)
        {
            return new DelegateCollectionImpl(metadata.Select(d => new DelegateImpl(d, parent)));
        }
    }
}