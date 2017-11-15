using System;
using System.Collections.Generic;

namespace Typewriter.Metadata.Interfaces
{
    public interface ITypeMetadata : IClassMetadata
    {
        bool IsEnum { get; }
        bool IsEnumerable { get; }
        bool IsNullable { get; }
        bool IsTask { get; }
        bool IsDefined { get; }
        bool IsValueTuple { get; }
        IEnumerable<IFieldMetadata> TupleElements { get; }
    }
}