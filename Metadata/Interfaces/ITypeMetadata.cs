using System;
using System.Collections.Generic;

namespace Typewriter.Metadata.Interfaces
{
    public interface ITypeMetadata : INamedItem
    {
        bool IsEnum { get; }
        bool IsEnumerable { get; }
        bool IsNullable { get; }
        bool IsGeneric { get; }
        string Namespace { get; }
        IEnumerable<IAttributeMetadata> Attributes { get; }
        IEnumerable<ITypeMetadata> TypeArguments { get; }
    }
}