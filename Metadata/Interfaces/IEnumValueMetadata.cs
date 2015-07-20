using System.Collections.Generic;

namespace Typewriter.Metadata.Interfaces
{
    public interface IEnumValueMetadata : INamedItem
    {
        IEnumerable<IAttributeMetadata> Attributes { get; }
        int Value { get; }
    }
}