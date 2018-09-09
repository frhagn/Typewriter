using System.Collections.Generic;

namespace Typewriter.Metadata.Interfaces
{
    public interface IAttributeMetadata : INamedItem
    {
        string Value { get; }
        IEnumerable<IAttributeArgumentMetadata> Arguments { get; }
    }
}