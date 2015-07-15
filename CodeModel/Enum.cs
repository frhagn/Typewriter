using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents an enum.
    /// </summary>
    [Context("Enum", "Enums")]
    public interface Enum : CodeItem
    {
        /// <summary>
        /// The namespace of the enum.
        /// </summary>
        [Property("string Namespace", "The namespace of the $context")]
        string Namespace { get; }

        /// <summary>
        /// Determines if the enum is decorated with the Flags attribute.
        /// </summary>
        [Property("bool IsFlags", "Determines if the $context is decorated with the Flags attribute")]
        bool IsFlags { get; }

        /// <summary>
        /// All values defined in the enum.
        /// </summary>
        [Property("collection Values", "All values defined in the $context")]
        EnumValueCollection Values { get; }

        /// <summary>
        /// The containing class of the enum if it is nested.
        /// </summary>
        [Property("class ContainingClass", "The containing class of the $context if it is nested")]
        Class ContainingClass { get; }
    }

    /// <summary>
    /// Represents a collection of enums.
    /// </summary>
    public interface EnumCollection : ItemCollection<Enum>
    {
    }
}