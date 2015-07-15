using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a field.
    /// </summary>
    [Context("Field", "Fields")]
    public interface Field : CodeItem
    {
        //[Property("bool IsEnum", "Determines if the $context is an enum")]
        //bool IsEnum { get; }

        //[Property("bool IsEnumerable", "Determines if the $context is enumerable")]
        //bool IsEnumerable { get; }

        //[Property("bool IsGeneric", "Determines if the $context is generic")]
        //bool IsGeneric { get; }

        //[Property("bool IsNullable", "Determines if the $context is nullable")]
        //bool IsNullable { get; }

        //[Property("bool IsPrimitive", "Determines if the $context is primitive")]
        //bool IsPrimitive { get; }

        /// <summary>
        /// The type of the field
        /// </summary>
        [Property("type Type", "The type of the $context")]
        Type Type { get; }
    }

    /// <summary>
    /// Represents a collection of fields.
    /// </summary>
    public interface FieldCollection : ItemCollection<Field>
    {
    }
}