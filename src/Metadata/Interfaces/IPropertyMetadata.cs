using System;

namespace Typewriter.Metadata.Interfaces
{
    public interface IPropertyMetadata : IFieldMetadata
    {
        bool IsAbstract { get; }
        bool HasGetter { get; }
        bool HasSetter { get; }
    }
}