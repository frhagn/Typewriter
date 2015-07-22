using System;
using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;
using static Typewriter.CodeModel.Helpers;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class TypeImpl : Type
    {
        private readonly ITypeMetadata metadata;
        private readonly Lazy<string> lazyName;

        private TypeImpl(ITypeMetadata metadata, Item parent)
        {
            this.metadata = metadata;
            this.Parent = parent;
            this.lazyName = new Lazy<string>(() => GetTypeScriptName(metadata));
        }

        public override Item Parent { get; }

        public override string name => CamelCase(lazyName.Value);
        public override string Name => lazyName.Value;
        public override string OriginalName => metadata.Name;
        public override string FullName => metadata.FullName;
        public override string Namespace => metadata.Namespace;
        public override bool IsGeneric => metadata.IsGeneric;
        public override bool IsEnum => metadata.IsEnum;
        public override bool IsEnumerable => metadata.IsEnumerable;
        public override bool IsNullable => metadata.IsNullable;
        public override bool IsPrimitive => metadata.IsPrimitive;
        public override bool IsDate => Name == "Date";
        public override bool IsGuid => FullName == "System.Guid" || FullName == "System.Guid?";
        public override bool IsTimeSpan => FullName == "System.TimeSpan" || FullName == "System.TimeSpan?";
        public override string Default => GetDefaultValue();

        private AttributeCollection attributes;
        public override AttributeCollection Attributes => attributes ?? (attributes = AttributeImpl.FromMetadata(metadata.Attributes, this));

        private ConstantCollection constants;
        public override ConstantCollection Constants => constants ?? (constants = ConstantImpl.FromMetadata(metadata.Constants, this));

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

        private TypeCollection genericTypeArguments;
        public override TypeCollection GenericTypeArguments => genericTypeArguments ?? (genericTypeArguments = TypeImpl.FromMetadata(metadata.GenericTypeArguments, this));

        private ClassCollection nestedClasses;
        public override ClassCollection NestedClasses => nestedClasses ?? (nestedClasses = ClassImpl.FromMetadata(metadata.NestedClasses, this));

        private EnumCollection nestedEnums;
        public override EnumCollection NestedEnums => nestedEnums ?? (nestedEnums = EnumImpl.FromMetadata(metadata.NestedEnums, this));

        private InterfaceCollection nestedInterfaces;
        public override InterfaceCollection NestedInterfaces => nestedInterfaces ?? (nestedInterfaces = InterfaceImpl.FromMetadata(metadata.NestedInterfaces, this));
        
        private string GetDefaultValue()
        {
            // Dictionary = { [key: type]: type; }
            if (Name.StartsWith("{")) return "{}";

            if (IsEnumerable) return "[]";

            if (Name == "boolean") return "false";
            if (Name == "number") return "0";
            if (Name == "void") return "void(0)";

            return "null";
        }

        public override string ToString()
        {
            return Name;
        }

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