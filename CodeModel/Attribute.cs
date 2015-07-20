using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents an attribute.
    /// </summary>
    [Context("Attribute", "Attributes")]
    public abstract class Attribute : CodeItem
    {
        /// <summary>
        /// The name of the attribute (camelCased).
        /// </summary>
        [Property("string name", "The name of the $context (camelCased).")]
        public abstract string name { get; }

        /// <summary>
        /// The name of the attribute.
        /// </summary>
        [Property("string Name", "The name of the $context.")]
        public abstract string Name { get; }

        /// <summary>
        /// The full original name of the attribute including namespace and containing class names.
        /// </summary>
        [Property("string FullName", "The full original name of the $context including namespace and containing class names.")]
        public abstract string FullName { get; }

        /// <summary>
        /// The value of the attribute as string.
        /// </summary>
        [Property("string Value", "The value of the $context as string.")]
        public abstract string Value { get; }
    }

    /// <summary>
    /// Represents a collection of attributes.
    /// </summary>
    public interface AttributeCollection : ItemCollection<Attribute>
    {
    }
}