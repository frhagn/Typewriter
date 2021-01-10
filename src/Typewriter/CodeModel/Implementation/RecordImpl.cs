using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;
using static Typewriter.CodeModel.Helpers;

namespace Typewriter.CodeModel.Implementation
{
    public class RecordImpl : Record
    {
        private readonly IRecordMetadata _metadata;

        private RecordImpl(IRecordMetadata metadata, Item parent)
        {
            _metadata = metadata;
            Parent = parent;
        }

        private AttributeCollection _attributes;
        public override AttributeCollection Attributes => _attributes ?? (_attributes = AttributeImpl.FromMetadata(_metadata.Attributes, this));
        private Record _baseRecord;
        public override Record BaseRecord => _baseRecord ?? (_baseRecord = RecordImpl.FromMetadata(_metadata.BaseRecord, this));
        private ConstantCollection _constants;
        public override ConstantCollection Constants => _constants ?? (_constants = ConstantImpl.FromMetadata(_metadata.Constants, this));
        private Record _containingRecord;
        public override Record ContainingRecord => _containingRecord ?? (_containingRecord = RecordImpl.FromMetadata(_metadata.ContainingRecord, this));
        private DelegateCollection _delegates;
        public override DelegateCollection Delegates => _delegates ?? (_delegates = DelegateImpl.FromMetadata(_metadata.Delegates, this));
        private DocComment _docComment;
        public override DocComment DocComment => _docComment ?? (_docComment = DocCommentImpl.FromXml(_metadata.DocComment, this));
        private EventCollection _events;
        public override EventCollection Events => _events ?? (_events = EventImpl.FromMetadata(_metadata.Events, this));
        private FieldCollection _fields;
        public override FieldCollection Fields => _fields ?? (_fields = FieldImpl.FromMetadata(_metadata.Fields, this));
        public override string FullName => _metadata.FullName;
        private InterfaceCollection _interfaces;
        public override InterfaceCollection Interfaces => _interfaces ?? (_interfaces = InterfaceImpl.FromMetadata(_metadata.Interfaces, this));
        public override bool IsAbstract => _metadata.IsAbstract;
        public override bool IsGeneric => _metadata.IsGeneric;
        private MethodCollection _methods;
        public override MethodCollection Methods => _methods ?? (_methods = MethodImpl.FromMetadata(_metadata.Methods, this));
        public override string name  => CamelCase(_metadata.Name.TrimStart('@'));
        public override string Name => _metadata.Name.TrimStart('@');
        public override string Namespace => _metadata.Namespace;
        public override Item Parent { get; }
        private PropertyCollection _properties;
        public override PropertyCollection Properties => _properties ?? (_properties = PropertyImpl.FromMetadata(_metadata.Properties, this));
        private TypeParameterCollection _typeParameters;
        public override TypeParameterCollection TypeParameters => _typeParameters ?? (_typeParameters = TypeParameterImpl.FromMetadata(_metadata.TypeParameters, this));
        private TypeCollection _typeArguments;
        public override TypeCollection TypeArguments => _typeArguments ?? (_typeArguments = TypeImpl.FromMetadata(_metadata.TypeArguments, this));
        private Type _type;
        protected override Type Type => _type ?? (_type = TypeImpl.FromMetadata(_metadata.Type, Parent));
        public override string ToString()
        {
            return Name;
        }

        public static RecordCollection FromMetadata(IEnumerable<IRecordMetadata> metadata, Item parent)
        {
            return new RecordCollectionImpl(metadata.Select(c => new RecordImpl(c, parent)));
        }

        public static Record FromMetadata(IRecordMetadata metadata, Item parent)
        {
            return metadata == null ? null : new RecordImpl(metadata, parent);
        }
    }
}