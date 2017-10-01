using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a method.
    /// </summary>
    [Context("Method", "Methods")]
    public abstract class Method : Item
    {
        /// <summary>
        /// All attributes defined on the method.
        /// </summary>
        public abstract AttributeCollection Attributes { get; }

        /// <summary>
        /// The XML documentation comment of the method.
        /// </summary>
        public abstract DocComment DocComment { get; }

        /// <summary>
        /// The full original name of the method including namespace and containing class names.
        /// </summary>
        public abstract string FullName { get; }

        /// <summary>
        /// Determines if the method is abstract.
        /// </summary>
        public abstract bool IsAbstract { get; }

        /// <summary>
        /// Determines if the method is generic.
        /// </summary>
        public abstract bool IsGeneric { get; }

        /// <summary>
        /// The name of the method (camelCased).
        /// </summary>
        public abstract string name { get; }

        /// <summary>
        /// The name of the method.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// All parameters of the method.
        /// </summary>
        public abstract ParameterCollection Parameters { get; }

        /// <summary>
        /// The parent context of the method.
        /// </summary>
        public abstract Item Parent { get; }

        /// <summary>
        /// The type of the method.
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        /// All generic type parameters of the method.
        /// TypeParameters are the type placeholders of a generic method e.g. &lt;T&gt;.
        /// </summary>
        public abstract TypeParameterCollection TypeParameters { get; }

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