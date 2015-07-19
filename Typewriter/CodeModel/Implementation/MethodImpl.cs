using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class MethodImpl : Method
    {
        private readonly IMethodMetadata metadata;
        private readonly Item parent;

        private MethodImpl(IMethodMetadata metadata, Item parent)
        {
            this.metadata = metadata;
            this.parent = parent;
        }

        public Item Parent => parent;
        public string Name => metadata.Name;
        public string FullName => metadata.FullName;
        public bool IsGeneric => metadata.IsGeneric;

        private AttributeCollection attributes;
        public AttributeCollection Attributes => attributes ?? (attributes = AttributeImpl.FromMetadata(metadata.Attributes, this));

        private TypeCollection genericTypeArguments;
        public TypeCollection GenericTypeArguments => genericTypeArguments ?? (genericTypeArguments = TypeImpl.FromMetadata(metadata.GenericTypeArguments, this));

        private ParameterCollection parameters;
        public ParameterCollection Parameters => parameters ?? (parameters = ParameterImpl.FromMetadata(metadata.Parameters, this));

        private Type type;
        public Type Type => type ?? (type = TypeImpl.FromMetadata(metadata.Type, this));
        
        public static MethodCollection FromMetadata(IEnumerable<IMethodMetadata> metadata, Item parent)
        {
            return new MethodCollectionImpl(metadata.Select(m => new MethodImpl(m, parent)));
        }
    }
}