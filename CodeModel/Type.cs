using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a type.
    /// </summary>
    [Context("Type", "Types")]
    public interface Type : Class
    {
        /// <summary>
        /// Determines if the type is an enum.
        /// </summary>
        [Property("bool IsEnum", "Determines if the $context is an enum")]
        bool IsEnum { get; }

        /// <summary>
        /// Determines if the type is enumerable.
        /// </summary>
        [Property("bool IsEnumerable", "Determines if the $context is enumerable")]
        bool IsEnumerable { get; }

        /// <summary>
        /// Determines if the type is nullable.
        /// </summary>
        [Property("bool IsNullable", "Determines if the $context is nullable")]
        bool IsNullable { get; }

        /// <summary>
        /// Determines if the type is primitive.
        /// </summary>
        [Property("bool IsPrimitive", "Determines if the $context is primitive")]
        bool IsPrimitive { get; }
    }

    /// <summary>
    /// Represents a collection of types.
    /// </summary>
    public interface TypeCollection : ItemCollection<Type>
    {
    }
}