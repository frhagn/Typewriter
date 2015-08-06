using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents an interface.
    /// </summary>
    [Context("Interface", "Interfaces")]
    public abstract class Interface : CodeItem
    {
        /// <summary>
        /// The name of the interface (camelCased).
        /// </summary>
        [Property("string name", "The name of the $context (camelCased).")]
        public abstract string name { get; }

        /// <summary>
        /// The name of the interface.
        /// </summary>
        [Property("string Name", "The name of the $context.")]
        public abstract string Name { get; }

        /// <summary>
        /// The full original name of the interface including namespace and containing class names.
        /// </summary>
        [Property("string FullName", "The full original name of the $context including namespace and containing class names.")]
        public abstract string FullName { get; }

        /// <summary>
        /// The namespace of the interface.
        /// </summary>
        [Property("string Namespace", "The namespace of the $context.")]
        public abstract string Namespace { get; }

        /// <summary>
        /// Determines if the interface is generic.
        /// </summary>
        [Property("bool IsGeneric", "Determines if the $context is generic.")]
        public abstract bool IsGeneric { get; }

        /// <summary>
        /// All attributes defined on the interface.
        /// </summary>
        [Property("collection Attributes", "All attributes defined on the $context.")]
        public abstract AttributeCollection Attributes { get; }

        /// <summary>
        /// All generic type parameters of the interface.
        /// </summary>
        [Property("collection TypeParameters", "All generic type parameters of the $context.", requireTemplate: false)]
        public abstract TypeParameterCollection TypeParameters { get; }

        /// <summary>
        /// All interfaces implemented by the interface.
        /// </summary>
        [Property("collection Interfaces", "All interfaces implemented by the $context.")]
        public abstract InterfaceCollection Interfaces { get; }

        /// <summary>
        /// All methods defined in the interface.
        /// </summary>
        [Property("collection Methods", "All methods defined in the $context.")]
        public abstract MethodCollection Methods { get; }

        /// <summary>
        /// All properties defined in the interface.
        /// </summary>
        [Property("collection Properties", "All properties defined in the $context.")]
        public abstract PropertyCollection Properties { get; }

        /// <summary>
        /// The containing class of the interface if it is nested.
        /// </summary>
        [Property("class ContainingClass", "The containing class of the $context if it is nested.")]
        public abstract Class ContainingClass { get; }

        /// <summary>
        /// Converts the current instance to string.
        /// </summary>
        public static implicit operator string (Interface instance)
        {
            return instance.ToString();
        }
    }

    /// <summary>
    /// Represents a collection of interfaces.
    /// </summary>
    public interface InterfaceCollection : ItemCollection<Interface>
    {
    }
}