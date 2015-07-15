using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents an enum value.
    /// </summary>
    [Context("EnumValue", "Values")]
    public interface EnumValue : CodeItem
    {
        /// <summary>
        /// The numeric value.
        /// </summary>
        [Property("number Value", "The numeric value")]
        int Value { get; }
    }

    /// <summary>
    /// Represents a collection of enum values.
    /// </summary>
    public interface EnumValueCollection : ItemCollection<EnumValue>
    {
    }
}