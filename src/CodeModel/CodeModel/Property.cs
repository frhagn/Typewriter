using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a property.
    /// </summary>
    [Context("Property", "Properties")]
    public abstract class Property : Item
    {
        /// <summary>
        /// All attributes defined on the property.
        /// </summary>
        public abstract AttributeCollection Attributes { get; }

        /// <summary>
        /// The XML documentation comment of the property.
        /// </summary>
        public abstract DocComment DocComment { get; }

        /// <summary>
        /// The full original name of the property including namespace and containing class names.
        /// </summary>
        public abstract string FullName { get; }

        /// <summary>
        /// Determines if the property has a getter.
        /// </summary>
        public abstract bool HasGetter { get; }

        /// <summary>
        /// Determines if the property has a setter.
        /// </summary>
        public abstract bool HasSetter { get; }

        /// <summary>
        /// Determines if the property is abstract.
        /// </summary>
        public abstract bool IsAbstract { get; }

        /// <summary>
        /// The name of the property (camelCased).
        /// </summary>
        public abstract string name { get; }

        /// <summary>
        /// The name of the property.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The parent context of the property.
        /// </summary>
        public abstract Item Parent { get; }

        /// <summary>
        /// The type of the property.
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        /// Converts the current instance to string.
        /// </summary>
        public static implicit operator string (Property instance)
        {
            return instance.ToString();
        }
    }

    /// <summary>
    /// Represents a collection of properties.
    /// </summary>
    public interface PropertyCollection : ItemCollection<Property>
    {
    }
}