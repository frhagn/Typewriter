using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class InterfaceImpl : Interface
    {
        private readonly IInterfaceMetadata metadata;
        private readonly Item parent;

        private InterfaceImpl(IInterfaceMetadata metadata, Item parent)
        {
            this.metadata = metadata;
            this.parent = parent;
        }

        public Item Parent => parent;
        public string Name => metadata.Name;
        public string FullName => metadata.FullName;
        public string Namespace => metadata.Namespace;
        public bool IsGeneric => metadata.IsGeneric;
        
        private AttributeCollection attributes;
        public AttributeCollection Attributes => attributes ?? (attributes = AttributeImpl.FromMetadata(metadata.Attributes, this));

        private InterfaceCollection interfaces;
        public InterfaceCollection Interfaces => interfaces ?? (interfaces = InterfaceImpl.FromMetadata(metadata.Interfaces, this));

        private MethodCollection methods;
        public MethodCollection Methods => methods ?? (methods = MethodImpl.FromMetadata(metadata.Methods, this));

        private PropertyCollection properties;
        public PropertyCollection Properties => properties ?? (properties = PropertyImpl.FromMetadata(metadata.Properties, this));

        private TypeCollection genericTypeArguments;
        public TypeCollection GenericTypeArguments => genericTypeArguments ?? (genericTypeArguments = TypeImpl.FromMetadata(metadata.GenericTypeArguments, this));

        private Class containingClass;
        public Class ContainingClass => containingClass ?? (containingClass = ClassImpl.FromMetadata(metadata.ContainingClass, this));

        public static InterfaceCollection FromMetadata(IEnumerable<IInterfaceMetadata> metadata, Item parent)
        {
            return new InterfaceCollectionImpl(metadata.Select(i => new InterfaceImpl(i, parent)));
        }
    }
}