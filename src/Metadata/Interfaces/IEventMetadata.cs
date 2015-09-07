using System.Collections.Generic;

namespace Typewriter.Metadata.Interfaces
{
    public interface IEventMetadata : INamedItem
    {
        IEnumerable<IAttributeMetadata> Attributes { get; }
        ITypeMetadata Type { get; }
    }
}