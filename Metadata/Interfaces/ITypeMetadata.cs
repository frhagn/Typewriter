using System;

namespace Typewriter.Metadata.Interfaces
{
    public interface ITypeMetadata : IClassMetadata
    {
        bool IsEnum { get; }
        bool IsEnumerable { get; }
        bool IsNullable { get; }
        bool IsPrimitive { get; }
    }
}