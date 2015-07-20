using System.Collections.Generic;

namespace Typewriter.Metadata.Interfaces
{
    public interface IEnumMetadata : INamedItem
    {
        string Namespace { get; }
        IEnumerable<IAttributeMetadata> Attributes { get; }
        IClassMetadata ContainingClass { get; }
        IEnumerable<IEnumValueMetadata> Values { get; }
    }
}