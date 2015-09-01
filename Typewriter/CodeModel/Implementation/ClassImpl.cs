using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;
using static Typewriter.CodeModel.Helpers;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class ClassImpl : Class
    {
        private readonly IClassMetadata metadata;

        private ClassImpl(IClassMetadata metadata, Item parent)
        {
            this.metadata = metadata;
            this.Parent = parent;
        }

        public override Item Parent { get; }

        public override string name => CamelCase(metadata.Name);
        public override string Name => metadata.Name;
        public override string FullName => metadata.FullName;
        public override string Namespace => metadata.Namespace;
        public override bool IsGeneric => metadata.IsGeneric;

        private AttributeCollection attributes;
        public override AttributeCollection Attributes => attributes ?? (attributes = AttributeImpl.FromMetadata(metadata.Attributes, this));

        private ConstantCollection constants;
        public override ConstantCollection Constants => constants ?? (constants = ConstantImpl.FromMetadata(metadata.Constants, this));

        private DelegateCollection delegates;
        public override DelegateCollection Delegates => delegates ?? (delegates = DelegateImpl.FromMetadata(metadata.Delegates, this));

        private FieldCollection fields;
        public override FieldCollection Fields => fields ?? (fields = FieldImpl.FromMetadata(metadata.Fields, this));

        private Class baseClass;
        public override Class BaseClass => baseClass ?? (baseClass = ClassImpl.FromMetadata(metadata.BaseClass, this));

        private Class containingClass;
        public override Class ContainingClass => containingClass ?? (containingClass = ClassImpl.FromMetadata(metadata.ContainingClass, this));

        private InterfaceCollection interfaces;
        public override InterfaceCollection Interfaces => interfaces ?? (interfaces = InterfaceImpl.FromMetadata(metadata.Interfaces, this));

        private MethodCollection methods;
        public override MethodCollection Methods => methods ?? (methods = MethodImpl.FromMetadata(metadata.Methods, this));

        private PropertyCollection properties;
        public override PropertyCollection Properties => properties ?? (properties = PropertyImpl.FromMetadata(metadata.Properties, this));

        private TypeParameterCollection typeParameters;
        public override TypeParameterCollection TypeParameters => typeParameters ?? (typeParameters = TypeParameterImpl.FromMetadata(metadata.TypeParameters, this));

        private TypeCollection typeArguments;
        public override TypeCollection TypeArguments => typeArguments ?? (typeArguments = TypeImpl.FromMetadata(metadata.TypeArguments, this));

        private ClassCollection nestedClasses;
        public override ClassCollection NestedClasses => nestedClasses ?? (nestedClasses = ClassImpl.FromMetadata(metadata.NestedClasses, this));

        private EnumCollection nestedEnums;
        public override EnumCollection NestedEnums => nestedEnums ?? (nestedEnums = EnumImpl.FromMetadata(metadata.NestedEnums, this));

        private InterfaceCollection nestedInterfaces;
        public override InterfaceCollection NestedInterfaces => nestedInterfaces ?? (nestedInterfaces = InterfaceImpl.FromMetadata(metadata.NestedInterfaces, this));

        public override string ToString()
        {
            return Name;
        }

        public static ClassCollection FromMetadata(IEnumerable<IClassMetadata> metadata, Item parent)
        {
            return new ClassCollectionImpl(metadata.Select(c => new ClassImpl(c, parent)));
        }

        public static Class FromMetadata(IClassMetadata metadata, Item parent)
        {
            return metadata == null ? null : new ClassImpl(metadata, parent);
        }
    }
}
