using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a field.
    /// </summary>
    [Context("Field", "Fields")]
    public abstract class Field : CodeItem
    {
        /// <summary>
        /// The name of the field (camelCased).
        /// </summary>
        [Property("string name", "The name of the $context (camelCased).")]
        public abstract string name { get; }

        /// <summary>
        /// The name of the field.
        /// </summary>
        [Property("string Name", "The name of the $context.")]
        public abstract string Name { get; }

        /// <summary>
        /// The full original name of the field including namespace and containing class names.
        /// </summary>
        [Property("string FullName", "The full original name of the $context including namespace and containing class names.")]
        public abstract string FullName { get; }

        /// <summary>
        /// All attributes defined on the field.
        /// </summary>
        [Property("collection Attributes", "All attributes defined on the $context.")]
        public abstract AttributeCollection Attributes { get; }

        /// <summary>
        /// The type of the field
        /// </summary>
        [Property("type Type", "The type of the $context.")]
        public abstract Type Type { get; }
    }

    /// <summary>
    /// Represents a collection of fields.
    /// </summary>
    public interface FieldCollection : ItemCollection<Field>
    {
    }
}