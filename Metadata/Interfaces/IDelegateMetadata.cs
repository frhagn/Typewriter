using System;

namespace Typewriter.Metadata.Interfaces
{
    public interface IDelegateMetadata : IMethodMetadata
    {
        IClassMetadata ContainingClass { get; }
    }
}