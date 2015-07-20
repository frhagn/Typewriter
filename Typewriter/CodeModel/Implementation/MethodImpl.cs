using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;
using static Typewriter.CodeModel.Helpers;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class MethodImpl : Method
    {
        private readonly IMethodMetadata metadata;

        private MethodImpl(IMethodMetadata metadata, Item parent)
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

        private TypeCollection genericTypeArguments;
        public override TypeCollection GenericTypeArguments => genericTypeArguments ?? (genericTypeArguments = TypeImpl.FromMetadata(metadata.GenericTypeArguments, this));

        private ParameterCollection parameters;
        public override ParameterCollection Parameters => parameters ?? (parameters = ParameterImpl.FromMetadata(metadata.Parameters, this));

        private Type type;
        public override Type Type => type ?? (type = TypeImpl.FromMetadata(metadata.Type, this));

        public override string ToString()
        {
            return Name;
        }

        public static MethodCollection FromMetadata(IEnumerable<IMethodMetadata> metadata, Item parent)
        {
            return new MethodCollectionImpl(metadata.Select(m => new MethodImpl(m, parent)));
        }
    }
}