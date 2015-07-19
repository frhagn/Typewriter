using System.Collections.Generic;

namespace Typewriter.Metadata.Interfaces
{
    public interface IFieldMetadata
    {
        string Name { get; }
        string FullName { get; }
        IEnumerable<IAttributeMetadata> Attributes { get; }
        ITypeMetadata Type { get; }
    }
}