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
        private readonly ITypeMetadata _metadata;
        private readonly Lazy<string> _lazyName;
        private readonly Lazy<string> _lazyOriginalName;

        private TypeImpl(ITypeMetadata metadata, Item parent)
        {
            _metadata = metadata;
            Parent = parent;
            _lazyName = new Lazy<string>(() => GetTypeScriptName(metadata));
            _lazyOriginalName = new Lazy<string>(() => GetOriginalName(metadata));
        }

        public override Item Parent { get; }

        public override string name => CamelCase(_lazyName.Value.TrimStart('@'));
        public override string Name => _lazyName.Value.TrimStart('@');
        public override string OriginalName => _lazyOriginalName.Value;
        public override string FullName => _metadata.FullName;
        public override string Namespace => _metadata.Namespace;
        public override bool IsGeneric => _metadata.IsGeneric;
        public override bool IsEnum => _metadata.IsEnum;
        public override bool IsEnumerable => _metadata.IsEnumerable;
        public override bool IsNullable => _metadata.IsNullable;
        public override bool IsTask => _metadata.IsTask;
        public override bool IsPrimitive => IsPrimitive(_metadata);
        public override bool IsDate => Name == "Date";
        public override bool IsDefined => _metadata.IsDefined;
        public override bool IsGuid => FullName == "System.Guid" || FullName == "System.Guid?";
        public override bool IsTimeSpan => FullName == "System.TimeSpan" || FullName == "System.TimeSpan?";
        public override bool IsValueTuple => _metadata.IsValueTuple;


        private AttributeCollection _attributes;
        public override AttributeCollection Attributes => _attributes ?? (_attributes = AttributeImpl.FromMetadata(_metadata.Attributes, this));

        private DocComment _docComment;
        public override DocComment DocComment => _docComment ?? (_docComment = DocCommentImpl.FromXml(_metadata.DocComment, this));

        private ConstantCollection _constants;
        public override ConstantCollection Constants => _constants ?? (_constants = ConstantImpl.FromMetadata(_metadata.Constants, this));

        private DelegateCollection _delegates;
        public override DelegateCollection Delegates => _delegates ?? (_delegates = DelegateImpl.FromMetadata(_metadata.Delegates, this));

        private FieldCollection _fields;
        public override FieldCollection Fields => _fields ?? (_fields = FieldImpl.FromMetadata(_metadata.Fields, this));

        private Class _baseClass;
        public override Class BaseClass => _baseClass ?? (_baseClass = ClassImpl.FromMetadata(_metadata.BaseClass, this));

        private Class _containingClass;
        public override Class ContainingClass => _containingClass ?? (_containingClass = ClassImpl.FromMetadata(_metadata.ContainingClass, this));

        private InterfaceCollection _interfaces;
        public override InterfaceCollection Interfaces => _interfaces ?? (_interfaces = InterfaceImpl.FromMetadata(_metadata.Interfaces, this));

        private MethodCollection _methods;
        public override MethodCollection Methods => _methods ?? (_methods = MethodImpl.FromMetadata(_metadata.Methods, this));

        private PropertyCollection _properties;
        public override PropertyCollection Properties => _properties ?? (_properties = PropertyImpl.FromMetadata(_metadata.Properties, this));

        private TypeCollection _typeArguments;
        public override TypeCollection TypeArguments => _typeArguments ?? (_typeArguments = TypeImpl.FromMetadata(_metadata.TypeArguments, this));

        private TypeParameterCollection _typeParameters;
        public override TypeParameterCollection TypeParameters => _typeParameters ?? (_typeParameters = TypeParameterImpl.FromMetadata(_metadata.TypeParameters, this));

        private FieldCollection _tupleElements;
        public override FieldCollection TupleElements => _tupleElements ?? (_tupleElements = FieldImpl.FromMetadata(_metadata.TupleElements, this));

        private ClassCollection _nestedClasses;
        public override ClassCollection NestedClasses => _nestedClasses ?? (_nestedClasses = ClassImpl.FromMetadata(_metadata.NestedClasses, this));

        private EnumCollection _nestedEnums;
        public override EnumCollection NestedEnums => _nestedEnums ?? (_nestedEnums = EnumImpl.FromMetadata(_metadata.NestedEnums, this));

        private InterfaceCollection _nestedInterfaces;
        public override InterfaceCollection NestedInterfaces => _nestedInterfaces ?? (_nestedInterfaces = InterfaceImpl.FromMetadata(_metadata.NestedInterfaces, this));
        
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