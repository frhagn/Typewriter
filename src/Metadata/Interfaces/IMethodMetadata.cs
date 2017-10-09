using System.Collections.Generic;

namespace Typewriter.Metadata.Interfaces
{
    public interface IMethodMetadata : IFieldMetadata
    {
        bool IsAbstract { get; }
        bool IsGeneric { get; }
        IEnumerable<ITypeParameterMetadata> TypeParameters { get; }
        IEnumerable<IParameterMetadata> Parameters { get; }
    }
}