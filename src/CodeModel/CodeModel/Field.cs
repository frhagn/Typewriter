using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a field.
    /// </summary>
    [Context("Field", "Fields")]
    public abstract class Field : Item
    {
        /// <summary>
        /// All attributes defined on the field.
        /// </summary>
        public abstract AttributeCollection Attributes { get; }

        /// <summary>
        /// The XML documentation comment of the field.
        /// </summary>
        public abstract DocComment DocComment { get; }

        /// <summary>
        /// The full original name of the field including namespace and containing class names.
        /// </summary>
        public abstract string FullName { get; }

        /// <summary>
        /// The name of the field (camelCased).
        /// </summary>
        public abstract string name { get; }

        /// <summary>
        /// The name of the field.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The parent context of the field.
        /// </summary>
        public abstract Item Parent { get; }

        /// <summary>
        /// The type of the field.
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        /// Converts the current instance to string.
        /// </summary>
        public static implicit operator string (Field instance)
        {
            return instance.ToString();
        }
    }

    /// <summary>
    /// Represents a collection of fields.
    /// </summary>
    public interface FieldCollection : ItemCollection<Field>
    {
    }
}