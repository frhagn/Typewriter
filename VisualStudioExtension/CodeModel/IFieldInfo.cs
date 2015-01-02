using System;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Field")]
    public interface IFieldInfo : IItemInfo
    {
        [Property("bool IsEnum", "Determines if the $context is an enum")]
        bool IsEnum { get; }

        [Property("bool IsEnumerable", "Determines if the $context is enumerable")]
        bool IsEnumerable { get; }
        
        [Property("bool IsGeneric", "Determines if the $context is generic")]
        bool IsGeneric { get; }

        [Property("bool IsNullable", "Determines if the $context is nullable")]
        bool IsNullable { get; }

        [Property("bool IsPrimitive", "Determines if the $context is primitive")]
        bool IsPrimitive { get; }
        
        [Property("string Class", "The class name of the $context")]
        string Class { get; }

        [Property("string Default", "The default value of the $context")]
        string Default { get; }

        [Property("type Type", "The type of the $context")]
        ITypeInfo Type { get; }
    }
}