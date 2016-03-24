using System.Collections.Generic;

namespace Typewriter.Metadata.Interfaces
{
    public interface IEnumValueMetadata : INamedItem
    {
        string DocComment { get; }
        IEnumerable<IAttributeMetadata> Attributes { get; }
        int Value { get; }
    }
}