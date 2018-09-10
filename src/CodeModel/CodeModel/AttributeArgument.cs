using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents an attribute argument.
    /// </summary>
    [Context("AttributeArgument", "Arguments")]
    public abstract class AttributeArgument : Item
    {
        /// <summary>
        /// The type of the argument.
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        /// The type value of the argument.
        /// </summary>
        public abstract Type TypeValue { get; }

        /// <summary>
        /// The value of the argument.
        /// </summary>
        public abstract object Value { get; }
    }

    /// <summary>
    /// Represents a collection of attribute arguments.
    /// </summary>
    public interface AttributeArgumentCollection : ItemCollection<AttributeArgument>
    {
    }
}