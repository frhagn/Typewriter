using System.Collections.Generic;

namespace Typewriter.Metadata.Interfaces
{
    public interface IParameterMetadata : INamedItem
    {
        IEnumerable<IAttributeMetadata> Attributes { get; }
        ITypeMetadata Type { get; }
    }
}