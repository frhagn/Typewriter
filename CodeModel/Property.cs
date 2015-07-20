using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a property.
    /// </summary>
    [Context("Property", "Properties")]
    public abstract class Property : Field
    {
        /// <summary>
        /// Determines if the property has a getter.
        /// </summary>
        [Property("bool HasGetter", "Determines if the $context has a getter")]
        public abstract bool HasGetter { get; }

        /// <summary>
        /// Determines if the property has a setter.
        /// </summary>
        [Property("bool HasSetter", "Determines if the $context has a setter")]
        public abstract bool HasSetter { get; }

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