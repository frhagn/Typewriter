using System.Collections.Generic;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.CodeDom
{
    internal class GenericTypeMetadata : ITypeMetadata
    {
        private readonly string fullName;

        public GenericTypeMetadata(string fullName)
        {
            this.fullName = fullName;
        }

        public string DocComment => null;
        public string FullName => fullName;
        public bool IsAbstract => false;
        public bool IsEnum => false;
        public bool IsEnumerable => false;
        public bool IsGeneric => false;
        public bool IsNullable => false;
        public bool IsTask => false;
        public bool IsDefined => false;
        public bool IsValueTuple => false;
        public string Name => fullName;
        public string Namespace => null;
        public ITypeMetadata Type => this;

        public IEnumerable<IAttributeMetadata> Attributes => new IAttributeMetadata[0];
        public IClassMetadata BaseClass => null;
        public IClassMetadata ContainingClass => null;
        public IEnumerable<IConstantMetadata> Constants => new IConstantMetadata[0];
        public IEnumerable<IDelegateMetadata> Delegates => new IDelegateMetadata[0];
        public IEnumerable<IEventMetadata> Events => new IEventMetadata[0];
        public IEnumerable<IFieldMetadata> Fields => new IFieldMetadata[0];
        public IEnumerable<IInterfaceMetadata> Interfaces => new IInterfaceMetadata[0];
        public IEnumerable<IMethodMetadata> Methods => new IMethodMetadata[0];
        public IEnumerable<IPropertyMetadata> Properties => new IPropertyMetadata[0];
        public IEnumerable<IClassMetadata> NestedClasses => new IClassMetadata[0];
        public IEnumerable<IEnumMetadata> NestedEnums => new IEnumMetadata[0];
        public IEnumerable<IInterfaceMetadata> NestedInterfaces => new IInterfaceMetadata[0];
        public IEnumerable<ITypeMetadata> TypeArguments => new ITypeMetadata[0];
        public IEnumerable<ITypeParameterMetadata> TypeParameters => new ITypeParameterMetadata[0];
        public IEnumerable<IFieldMetadata> TupleElements => new IFieldMetadata[0];
    }
}