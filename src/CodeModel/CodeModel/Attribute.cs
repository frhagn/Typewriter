using System.Collections.Generic;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents an attribute.
    /// </summary>
    [Context("Attribute", "Attributes")]
    public abstract class Attribute : Item
    {
        /// <summary>
        /// The full original name of the attribute including namespace and containing class names.
        /// </summary>
        public abstract string FullName { get; }

        /// <summary>
        /// The name of the attribute (camelCased).
        /// </summary>
        public abstract string name { get; }

        /// <summary>
        /// The name of the attribute.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The parent context of the attribute.
        /// </summary>
        public abstract Item Parent { get; }

        /// <summary>
        /// The value of the attribute as string.
        /// </summary>
        public abstract string Value { get; }

        /// <summary>
        /// The arguments of the attribute.
        /// </summary>
        public abstract AttributeArgumentCollection Arguments { get; }

        /// <summary>
        /// Converts the current instance to string.
        /// </summary>
        public static implicit operator string(Attribute instance)
        {
            return instance.ToString();
        }
    }

    /// <summary>
    /// Represents a collection of attributes.
    /// </summary>
    public interface AttributeCollection : ItemCollection<Attribute>
    {
    }
}