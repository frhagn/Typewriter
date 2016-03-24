using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a delegate.
    /// </summary>
    [Context("Delegate", "Delegates")]
    public abstract class Delegate : Item
    {
        /// <summary>
        /// All attributes defined on the delegate.
        /// </summary>
        public abstract AttributeCollection Attributes { get; }

        /// <summary>
        /// The XML documentation comment of the delegate.
        /// </summary>
        public abstract DocComment DocComment { get; }

        /// <summary>
        /// The full original name of the delegate including namespace and containing class names.
        /// </summary>
        public abstract string FullName { get; }

        /// <summary>
        /// Determines if the delegate is generic.
        /// </summary>
        public abstract bool IsGeneric { get; }

        /// <summary>
        /// The name of the delegate (camelCased).
        /// </summary>
        public abstract string name { get; }

        /// <summary>
        /// The name of the delegate.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// All parameters of the delegate.
        /// </summary>
        public abstract ParameterCollection Parameters { get; }

        /// <summary>
        /// The parent context of the delegate.
        /// </summary>
        public abstract Item Parent { get; }

        /// <summary>
        /// The type of the delegate.
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        /// All generic type parameters of the delegate.
        /// TypeParameters are the type placeholders of a generic delegate e.g. &lt;T&gt;.
        /// </summary>
        public abstract TypeParameterCollection TypeParameters { get; }

        /// <summary>
        /// Converts the current instance to string.
        /// </summary>
        public static implicit operator string (Delegate instance)
        {
            return instance.ToString();
        }
    }

    /// <summary>
    /// Represents a collection of classes.
    /// </summary>
    public interface DelegateCollection : ItemCollection<Delegate>
    {
    }
}
