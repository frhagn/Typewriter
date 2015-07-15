using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents ann item.
    /// </summary>
    public interface Item
    {
        /// <summary>
        /// The name of the item.
        /// </summary>
        [Property("string Name", "The name of the $context")]
        string Name { get; }

        /// <summary>
        /// The namespace and name of the item.
        /// </summary>
        [Property("string FullName", "The namespace and name of the $context")]
        string FullName { get; }
    }

    /// <summary>
    /// Represents a code item.
    /// </summary>
    public interface CodeItem : Item
    {
        /// <summary>
        /// The parent context ot the code item.
        /// </summary>
        [Property("$parent Parent", "The parent context of the $context")]
        Item Parent { get; }

        /// <summary>
        /// All attributes defined on the code item.
        /// </summary>
        [Property("collection Attributes", "All attributes defined on the $context")]
        AttributeCollection Attributes { get; }
    }
}