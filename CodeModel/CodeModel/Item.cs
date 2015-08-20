using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents an item.
    /// </summary>
    public abstract class Item
    {
    }

    /// <summary>
    /// Represents a code item.
    /// </summary>
    public abstract class CodeItem : Item
    {
        /// <summary>
        /// The parent context of the code item.
        /// </summary>
        //[Property("$parent Parent", "The parent context of the $context.")]
        public abstract Item Parent { get; }
    }
}