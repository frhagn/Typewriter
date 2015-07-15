using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents an attribute.
    /// </summary>
    [Context("Attribute", "Attributes")]
    public interface Attribute : CodeItem
    {
        /// <summary>
        /// The value of the attribute as string.
        /// </summary>
        [Property("string Value", "The value of the $context as string")]
        string Value { get; }
    }

    /// <summary>
    /// Represents a collection of attributes.
    /// </summary>
    public interface AttributeCollection : ItemCollection<Attribute>
    {
    }
}