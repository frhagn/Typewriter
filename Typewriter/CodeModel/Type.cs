using System;
using System.Collections.Generic;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Type", "Types")]
    public interface Type : Class
    {
        [Property("bool IsEnum", "Determines if the $context is an enum")]
        bool IsEnum { get; }

        [Property("bool IsEnumerable", "Determines if the $context is enumerable")]
        bool IsEnumerable { get; }

        [Property("bool IsNullable", "Determines if the $context is nullable")]
        bool IsNullable { get; }

        [Property("bool IsPrimitive", "Determines if the $context is primitive")]
        bool IsPrimitive { get; }

        [Property("collection GenericTypeArguments", "All generic type arguments of the $context")]
        IEnumerable<Type> GenericTypeArguments { get; }
    }
}