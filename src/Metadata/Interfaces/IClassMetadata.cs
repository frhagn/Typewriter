using System.Collections.Generic;

namespace Typewriter.Metadata.Interfaces
{
    public interface IClassMetadata : INamedItem
    {
        string DocComment { get; }
        bool IsAbstract { get; }
        bool IsGeneric { get; }
        string Namespace { get; }
        ITypeMetadata Type { get; }
        IEnumerable<IAttributeMetadata> Attributes { get; }
        IClassMetadata BaseClass { get; }
        IClassMetadata ContainingClass { get; }
        IEnumerable<IConstantMetadata> Constants { get; }
        IEnumerable<IDelegateMetadata> Delegates { get; }
        IEnumerable<IEventMetadata> Events { get; }
        IEnumerable<IFieldMetadata> Fields { get; }
        IEnumerable<IInterfaceMetadata> Interfaces { get; }
        IEnumerable<IMethodMetadata> Methods { get; }
        IEnumerable<IPropertyMetadata> Properties { get; }
        IEnumerable<ITypeParameterMetadata> TypeParameters { get; }
        IEnumerable<ITypeMetadata> TypeArguments { get; }
        IEnumerable<IClassMetadata> NestedClasses { get; }
        IEnumerable<IEnumMetadata> NestedEnums { get; }
        IEnumerable<IInterfaceMetadata> NestedInterfaces { get; }
    }
}