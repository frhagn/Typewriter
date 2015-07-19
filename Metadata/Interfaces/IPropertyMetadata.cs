using System;

namespace Typewriter.Metadata.Interfaces
{
    public interface IPropertyMetadata : IFieldMetadata
    {
        bool HasGetter { get; }
        bool HasSetter { get; }
    }
}