using System.Collections.Generic;

namespace Typewriter.Metadata.Interfaces
{
    public interface IEventMetadata : INamedItem
    {
        string DocComment { get; }
        IEnumerable<IAttributeMetadata> Attributes { get; }
        ITypeMetadata Type { get; }
    }
}