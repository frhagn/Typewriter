using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a property.
    /// </summary>
    [Context("Property", "Properties")]
    public interface Property : Field
    {
        /// <summary>
        /// Determines if the property has a getter.
        /// </summary>
        [Property("bool HasGetter", "Determines if the $context has a getter")]
        bool HasGetter { get; }

        /// <summary>
        /// Determines if the property has a setter.
        /// </summary>
        [Property("bool HasSetter", "Determines if the $context has a setter")]
        bool HasSetter { get; }
    }

    /// <summary>
    /// Represents a collection of properties.
    /// </summary>
    public interface PropertyCollection : ItemCollection<Property>
    {
    }
}