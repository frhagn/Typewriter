using System.Collections.Generic;

namespace Typewriter.Metadata.Interfaces
{
    public interface IAttributeMetadata
    {
        string Name { get; }
        string FullName { get; }
        string Value { get; }
    }
}