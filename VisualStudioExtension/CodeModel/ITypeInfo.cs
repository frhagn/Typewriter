using System;
using System.Collections.Generic;

namespace Typewriter.CodeModel
{
    public interface ITypeInfo : IClassInfo
    {
        bool IsEnum { get; }
        bool IsEnumerable { get; }
        bool IsNullable { get; }
        bool IsPrimitive { get; }
        IEnumerable<ITypeInfo> GenericTypeArguments { get; }
    }
}