using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class TypeImpl : Type
    {
        private readonly ITypeMetadata metadata;
        private readonly Item parent;

        private TypeImpl(ITypeMetadata metadata, Item parent)
        {
            this.metadata = metadata;
            this.parent = parent;
        }

        public Item Parent => parent;
        public string Name => metadata.Name;
        public string FullName => metadata.FullName;
        public string Namespace => metadata.Namespace;
        public bool IsGeneric => metadata.IsGeneric;
        public bool IsEnum => metadata.IsEnum;
        public bool IsEnumerable => metadata.IsEnumerable;
        public bool IsNullable => metadata.IsNullable;
        public bool IsPrimitive => metadata.IsPrimitive;

        private AttributeCollection attributes;
        public AttributeCollection Attributes => attributes ?? (attributes = AttributeImpl.FromMetadata(metadata.Attributes, this));

        private ConstantCollection constants;
        public ConstantCollection Constants => constants ?? (constants = ConstantImpl.FromMetadata(metadata.Constants, this));

        private FieldCollection fields;
        public FieldCollection Fields => fields ?? (fields = FieldImpl.FromMetadata(metadata.Fields, this));

        private Class baseClass;
        public Class BaseClass => baseClass ?? (baseClass = ClassImpl.FromMetadata(metadata.BaseClass, this));

        private Class containingClass;
        public Class ContainingClass => containingClass ?? (containingClass = ClassImpl.FromMetadata(metadata.ContainingClass, this));

        private InterfaceCollection interfaces;
        public InterfaceCollection Interfaces => interfaces ?? (interfaces = InterfaceImpl.FromMetadata(metadata.Interfaces, this));

        private MethodCollection methods;
        public MethodCollection Methods => methods ?? (methods = MethodImpl.FromMetadata(metadata.Methods, this));

        private PropertyCollection properties;
        public PropertyCollection Properties => properties ?? (properties = PropertyImpl.FromMetadata(metadata.Properties, this));

        private TypeCollection genericTypeArguments;
        public TypeCollection GenericTypeArguments => genericTypeArguments ?? (genericTypeArguments = TypeImpl.FromMetadata(metadata.GenericTypeArguments, this));

        private ClassCollection nestedClasses;
        public ClassCollection NestedClasses => nestedClasses ?? (nestedClasses = ClassImpl.FromMetadata(metadata.NestedClasses, this));

        private EnumCollection nestedEnums;
        public EnumCollection NestedEnums => nestedEnums ?? (nestedEnums = EnumImpl.FromMetadata(metadata.NestedEnums, this));

        private InterfaceCollection nestedInterfaces;
        public InterfaceCollection NestedInterfaces => nestedInterfaces ?? (nestedInterfaces = InterfaceImpl.FromMetadata(metadata.NestedInterfaces, this));

        public static TypeCollection FromMetadata(IEnumerable<ITypeMetadata> metadata, Item parent)
        {
            return new TypeCollectionImpl(metadata.Select(t => new TypeImpl(t, parent)));
        }

        public static Type FromMetadata(ITypeMetadata metadata, Item parent)
        {
            return metadata == null ? null : new TypeImpl(metadata, parent);
        }
    }
}