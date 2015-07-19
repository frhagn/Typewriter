using System.Collections.Generic;

namespace Typewriter.Metadata.Interfaces
{
    public interface IEnumMetadata
    {
        string Name { get; }
        string FullName { get; }
        string Namespace { get; }
        IEnumerable<IAttributeMetadata> Attributes { get; }
        IClassMetadata ContainingClass { get; }
        IEnumerable<IEnumValueMetadata> Values { get; }
    }
}