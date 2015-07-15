using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents an interface.
    /// </summary>
    [Context("Interface", "Interfaces")]
    public interface Interface : CodeItem
    {
        /// <summary>
        /// The namespace of the interface.
        /// </summary>
        [Property("string Namespace", "The namespace of the $context")]
        string Namespace { get; }

        /// <summary>
        /// Determines if the interface is generic.
        /// </summary>
        [Property("bool IsGeneric", "Determines if the $context is generic")]
        bool IsGeneric { get; }

        /// <summary>
        /// All generic type arguments of the interface.
        /// </summary>
        [Property("collection GenericTypeArguments", "All generic type arguments of the $context")]
        TypeCollection GenericTypeArguments { get; }

        /// <summary>
        /// All interfaces implemented by the interface.
        /// </summary>
        [Property("collection Interfaces", "All interfaces implemented by the $context")]
        InterfaceCollection Interfaces { get; }

        /// <summary>
        /// All methods defined in the interface.
        /// </summary>
        [Property("collection Methods", "All methods defined in the $context")]
        MethodCollection Methods { get; }

        /// <summary>
        /// All properties defined in the interface.
        /// </summary>
        [Property("collection Properties", "All properties defined in the $context")]
        PropertyCollection Properties { get; }

        /// <summary>
        /// The containing class of the interface if it is nested.
        /// </summary>
        [Property("class ContainingClass", "The containing class of the $context if it is nested")]
        Class ContainingClass { get; }
    }

    /// <summary>
    /// Represents a collection of interfaces.
    /// </summary>
    public interface InterfaceCollection : ItemCollection<Interface>
    {
    }
}