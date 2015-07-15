using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a method.
    /// </summary>
    [Context("Method", "Methods")]
    public interface Method : Field
    {
        /// <summary>
        /// Determines if the method is generic.
        /// </summary>
        [Property("bool IsGeneric", "Determines if the $context is generic")]
        bool IsGeneric { get; }

        /// <summary>
        /// All generic type arguments of the method.
        /// </summary>
        [Property("collection GenericTypeArguments", "All generic type arguments of the $context")]
        TypeCollection GenericTypeArguments { get; }

        /// <summary>
        /// All parameters of the method.
        /// </summary>
        [Property("collection Parameters", "All parameters of the $context")]
        ParameterCollection Parameters { get; }
    }

    /// <summary>
    /// Represents a collection of methods.
    /// </summary>
    public interface MethodCollection : ItemCollection<Method>
    {
    }
}