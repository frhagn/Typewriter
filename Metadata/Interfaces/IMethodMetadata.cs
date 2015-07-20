using System.Collections.Generic;

namespace Typewriter.Metadata.Interfaces
{
    public interface IMethodMetadata : IFieldMetadata
    {
        bool IsGeneric { get; }
        IEnumerable<ITypeMetadata> GenericTypeArguments { get; }
        IEnumerable<IParameterMetadata> Parameters { get; }
    }
}