using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents an enum.
    /// </summary>
    [Context("Enum", "Enums")]
    public abstract class Enum : CodeItem
    {
        /// <summary>
        /// The name of the enum (camelCased).
        /// </summary>
        [Property("string name", "The name of the $context (camelCased).")]
        public abstract string name { get; }

        /// <summary>
        /// The name of the enum.
        /// </summary>
        [Property("string Name", "The name of the $context.")]
        public abstract string Name { get; }

        /// <summary>
        /// The full original name of the enum including namespace and containing class names.
        /// </summary>
        [Property("string FullName", "The full original name of the $context including namespace and containing class names.")]
        public abstract string FullName { get; }

        /// <summary>
        /// The namespace of the enum.
        /// </summary>
        [Property("string Namespace", "The namespace of the $context.")]
        public abstract string Namespace { get; }

        /// <summary>
        /// Determines if the enum is decorated with the Flags attribute.
        /// </summary>
        [Property("bool IsFlags", "Determines if the $context is decorated with the Flags attribute.")]
        public abstract bool IsFlags { get; }

        /// <summary>
        /// All attributes defined on the enum.
        /// </summary>
        [Property("collection Attributes", "All attributes defined on the $context.")]
        public abstract AttributeCollection Attributes { get; }

        /// <summary>
        /// All values defined in the enum.
        /// </summary>
        [Property("collection Values", "All values defined in the $context.")]
        public abstract EnumValueCollection Values { get; }

        /// <summary>
        /// The containing class of the enum if it is nested.
        /// </summary>
        [Property("class ContainingClass", "The containing class of the $context if it is nested.")]
        public abstract Class ContainingClass { get; }
    }

    /// <summary>
    /// Represents a collection of enums.
    /// </summary>
    public interface EnumCollection : ItemCollection<Enum>
    {
    }
}