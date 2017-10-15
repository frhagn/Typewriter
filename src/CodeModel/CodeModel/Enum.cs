using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents an enum.
    /// </summary>
    [Context("Enum", "Enums")]
    public abstract class Enum : Item
    {
        /// <summary>
        /// All attributes defined on the enum.
        /// </summary>
        public abstract AttributeCollection Attributes { get; }

        /// <summary>
        /// The containing class of the enum if it is nested.
        /// </summary>
        public abstract Class ContainingClass { get; }

        /// <summary>
        /// The XML documentation comment of the enum.
        /// </summary>
        public abstract DocComment DocComment { get; }

        /// <summary>
        /// The full original name of the enum including namespace and containing class names.
        /// </summary>
        public abstract string FullName { get; }

        /// <summary>
        /// Determines if the enum is decorated with the Flags attribute.
        /// </summary>
        public abstract bool IsFlags { get; }

        /// <summary>
        /// The name of the enum (camelCased).
        /// </summary>
        public abstract string name { get; }

        /// <summary>
        /// The name of the enum.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The namespace of the enum.
        /// </summary>
        public abstract string Namespace { get; }

        /// <summary>
        /// The parent context of the enum.
        /// </summary>
        public abstract Item Parent { get; }

        /// <summary>
        /// All values defined in the enum.
        /// </summary>
        public abstract EnumValueCollection Values { get; }

        /// <summary>
        /// Converts the current instance to string.
        /// </summary>
        public static implicit operator string (Enum instance)
        {
            return instance.ToString();
        }

        protected abstract Type Type { get; }

        /// <summary>
        /// Converts the current instance to a Type.
        /// </summary>
        public static implicit operator Type(Enum instance)
        {
            return instance?.Type;
        }
    }

    /// <summary>
    /// Represents a collection of enums.
    /// </summary>
    public interface EnumCollection : ItemCollection<Enum>
    {
    }
}