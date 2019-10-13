using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents an enum value.
    /// </summary>
    [Context("EnumValue", "Values")]
    public abstract class EnumValue : Item
    {
        /// <summary>
        /// All attributes defined on the enum value.
        /// </summary>
        public abstract AttributeCollection Attributes { get; }

        /// <summary>
        /// The XML documentation comment of the enum value.
        /// </summary>
        public abstract DocComment DocComment { get; }

        /// <summary>
        /// The full original name of the enum value including namespace and containing class names.
        /// </summary>
        public abstract string FullName { get; }

        /// <summary>
        /// The name of the enum value (camelCased).
        /// </summary>
        public abstract string name { get; }

        /// <summary>
        /// The name of the enum value.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The parent context of the enum value.
        /// </summary>
        public abstract Item Parent { get; }

        /// <summary>
        /// The numeric value.
        /// </summary>
        public abstract long Value { get; }

        /// <summary>
        /// Converts the current instance to string.
        /// </summary>
        public static implicit operator string (EnumValue instance)
        {
            return instance.ToString();
        }
    }

    /// <summary>
    /// Represents a collection of enum values.
    /// </summary>
    public interface EnumValueCollection : ItemCollection<EnumValue>
    {
    }
}