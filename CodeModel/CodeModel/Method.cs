using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a method.
    /// </summary>
    [Context("Method", "Methods")]
    public abstract class Method : CodeItem
    {
        /// <summary>
        /// The name of the method (camelCased).
        /// </summary>
        //[Property("string name", "The name of the $context (camelCased).")]
        public abstract string name { get; }

        /// <summary>
        /// The name of the method.
        /// </summary>
        //[Property("string Name", "The name of the $context.")]
        public abstract string Name { get; }

        /// <summary>
        /// The full original name of the method including namespace and containing class names.
        /// </summary>
        //[Property("string FullName", "The full original name of the $context including namespace and containing class names.")]
        public abstract string FullName { get; }

        /// <summary>
        /// All attributes defined on the method.
        /// </summary>
        //[Property("collection Attributes", "All attributes defined on the $context.")]
        public abstract AttributeCollection Attributes { get; }

        /// <summary>
        /// Determines if the method is generic.
        /// </summary>
        //[Property("bool IsGeneric", "Determines if the $context is generic.")]
        public abstract bool IsGeneric { get; }

        /// <summary>
        /// All generic type parameters of the method.
        /// </summary>
        //[Property("collection TypeParameters", "All generic type parameters of the $context.", requireTemplate: false)]
        public abstract TypeParameterCollection TypeParameters { get; }

        /// <summary>
        /// All parameters of the method.
        /// </summary>
        //[Property("collection Parameters", "All parameters of the $context.")]
        public abstract ParameterCollection Parameters { get; }

        /// <summary>
        /// The type of the method.
        /// </summary>
        //[Property("type Type", "The type of the $context.")]
        public abstract Type Type { get; }

        /// <summary>
        /// Converts the current instance to string.
        /// </summary>
        public static implicit operator string (Method instance)
        {
            return instance.ToString();
        }
    }

    /// <summary>
    /// Represents a collection of methods.
    /// </summary>
    public interface MethodCollection : ItemCollection<Method>
    {
    }
}