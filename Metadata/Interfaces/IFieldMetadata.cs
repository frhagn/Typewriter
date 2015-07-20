using System.Collections.Generic;

namespace Typewriter.Metadata.Interfaces
{
    public interface IFieldMetadata : INamedItem
    {
        IEnumerable<IAttributeMetadata> Attributes { get; }
        ITypeMetadata Type { get; }
    }
}