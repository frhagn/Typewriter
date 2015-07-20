using System.Collections.Generic;

namespace Typewriter.Metadata.Interfaces
{
    public interface IInterfaceMetadata : INamedItem
    {
        IEnumerable<IAttributeMetadata> Attributes { get; }
        string Namespace { get; }
        bool IsGeneric { get; }
        IClassMetadata ContainingClass { get; }
        IEnumerable<ITypeMetadata> GenericTypeArguments { get; }
        IEnumerable<IInterfaceMetadata> Interfaces { get; }
        IEnumerable<IMethodMetadata> Methods { get; }
        IEnumerable<IPropertyMetadata> Properties { get; }
    }
}