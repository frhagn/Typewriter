using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents an enum value.
    /// </summary>
    [Context("EnumValue", "Values")]
    public abstract class EnumValue : CodeItem
    {
        /// <summary>
        /// The name of the enum value (camelCased).
        /// </summary>
        //[Property("string name", "The name of the $context (camelCased).")]
        public abstract string name { get; }

        /// <summary>
        /// The name of the enum value.
        /// </summary>
        //[Property("string Name", "The name of the $context.")]
        public abstract string Name { get; }

        /// <summary>
        /// The full original name of the enum value including namespace and containing class names.
        /// </summary>
        //[Property("string FullName", "The full original name of the $context including namespace and containing class names.")]
        public abstract string FullName { get; }

        /// <summary>
        /// The numeric value.
        /// </summary>
        //[Property("number Value", "The numeric value.")]
        public abstract int Value { get; }

        /// <summary>
        /// All attributes defined on the enum value.
        /// </summary>
        //[Property("collection Attributes", "All attributes defined on the $context.")]
        public abstract AttributeCollection Attributes { get; }
    }

    /// <summary>
    /// Represents a collection of enum values.
    /// </summary>
    public interface EnumValueCollection : ItemCollection<EnumValue>
    {
    }
}