using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a parameter.
    /// </summary>
    [Context("Parameter", "Parameters")]
    public abstract class Parameter : Item
    {
        /// <summary>
        /// All attributes defined on the parameter.
        /// </summary>
        public abstract AttributeCollection Attributes { get; }

        /// <summary>
        /// The full original name of the parameter.
        /// </summary>
        public abstract string FullName { get; }

        /// <summary>
        /// The name of the parameter (camelCased).
        /// </summary>
        public abstract string name { get; }

        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The parent context of the parameter.
        /// </summary>
        public abstract Item Parent { get; }

        /// <summary>
        /// The type of the parameter.
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        /// Converts the current instance to string.
        /// </summary>
        public static implicit operator string (Parameter instance)
        {
            return instance.ToString();
        }
    }

    /// <summary>
    /// Represents a collection of parameters.
    /// </summary>
    public interface ParameterCollection : ItemCollection<Parameter>
    {
    }
}