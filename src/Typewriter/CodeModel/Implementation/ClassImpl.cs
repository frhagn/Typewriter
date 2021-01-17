using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;
using static Typewriter.CodeModel.Helpers;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class ClassImpl : Class
    {
        private readonly IClassMetadata _metadata;

        private ClassImpl(IClassMetadata metadata, Item parent)
        {
            _metadata = metadata;
            Parent = parent;
        }

        public override Item Parent { get; }

        public override string name => CamelCase(_metadata.Name.TrimStart('@'));
        public override string Name => _metadata.Name.TrimStart('@');
        public override string FullName => _metadata.FullName;
        public override string Namespace => _metadata.Namespace;
        public override bool IsAbstract => _metadata.IsAbstract;
        public override bool IsGeneric => _metadata.IsGeneric;

        private Type _type;
        protected override Type Type => _type ?? (_type = TypeImpl.FromMetadata(_metadata.Type, Parent));
        
        private AttributeCollection _attributes;
        public override AttributeCollection Attributes => _attributes ?? (_attributes = AttributeImpl.FromMetadata(_metadata.Attributes, this));

        private ConstantCollection _constants;
        public override ConstantCollection Constants => _constants ?? (_constants = ConstantImpl.FromMetadata(_metadata.Constants, this));

        private DelegateCollection _delegates;
        public override DelegateCollection Delegates => _delegates ?? (_delegates = DelegateImpl.FromMetadata(_metadata.Delegates, this));

        private DocComment _docComment;
        public override DocComment DocComment => _docComment ?? (_docComment = DocCommentImpl.FromXml(_metadata.DocComment, this));

        private EventCollection _events;
        public override EventCollection Events => _events ?? (_events = EventImpl.FromMetadata(_metadata.Events, this));

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
        public override PropertyCollection Properties => _properties ?? (_properties = PropertyImpl.FromMetadata(GetPropertiesFromClassMetadata(_metadata.Properties), this));

        private TypeParameterCollection _typeParameters;
        public override TypeParameterCollection TypeParameters => _typeParameters ?? (_typeParameters = TypeParameterImpl.FromMetadata(_metadata.TypeParameters, this));

        private TypeCollection _typeArguments;
        public override TypeCollection TypeArguments => _typeArguments ?? (_typeArguments = TypeImpl.FromMetadata(_metadata.TypeArguments, this));

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

        public static ClassCollection FromMetadata(IEnumerable<IClassMetadata> metadata, Item parent)
        {
            return new ClassCollectionImpl(metadata.Select(c => new ClassImpl(c, parent)));
        }

        public static Class FromMetadata(IClassMetadata metadata, Item parent)
        {
            return metadata == null ? null : new ClassImpl(metadata, parent);
        }

        /** 
         *  Example of this:
         *  generated type:
         *  public partial class GeneratedClass
         *  {
         *      public string GeneratedProperty { get; set; }
         *  }
         *  user-defined type (in separate file):
         *  [ModelMetadata(typeof(GeneratedClassMetadata))]
         *  public partial class GeneratedClass
         *  {
         *
         *      internal sealed class GeneratedClassMetadata
         *      {
         *          [Column(DataType="varchar")]
         *          public string GeneratedProperty { get; set; }
         *      }
         *  }
         */
        /// <summary>
        /// Gets properties from a Metadata type: A nested class that allows decorators to be applied to generated code using either
        /// the MetadataType (.NET Framework) or ModelMetadataType (.NET Core) to link it to the original.
        /// </summary>
        /// <returns>If there is a metadata type defined, returns a collection of overridden properties merged with the originals.
        /// Otherwise, returns the original collection.</returns>
        internal IEnumerable<IPropertyMetadata> GetPropertiesFromClassMetadata(IEnumerable<IPropertyMetadata> originalProperties)
        {
            var classMetadata = _metadata.Attributes.FirstOrDefault(a => a.Name == "MetadataType" || a.Name == "ModelMetadataType");
            if (classMetadata == null)
            {
                return originalProperties;
            }

            var metadataTypeArgument = classMetadata.Arguments.First();
            var metadataType = metadataTypeArgument.TypeValue;
            if (metadataType == null)
            {
                return originalProperties;
            }
            //loop through the original properties and use the metadata property whenever it matches the name of an original
            var mergedProperties = new List<IPropertyMetadata>();
            foreach (var property in originalProperties)
            {
                var metadataProperty = metadataType.Properties
                    .FirstOrDefault(mp => mp.Name == property.Name);
                mergedProperties.Add(metadataProperty ?? property);
            }
            return mergedProperties;
        }
    }
}
