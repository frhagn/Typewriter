using System.Collections.Generic;

namespace Typewriter.Metadata.Interfaces
{
    public interface IEnumValueMetadata
    {
        string Name { get; }
        string FullName { get; }
        IEnumerable<IAttributeMetadata> Attributes { get; }
        int Value { get; }
    }
}