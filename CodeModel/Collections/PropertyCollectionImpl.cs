using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    /// <summary>
    /// Represents a collection of properties.
    /// </summary>
    public class PropertyCollectionImpl : List<Property>, PropertyCollection
    {
        /// <summary>
        /// Creates a new instance with no items.
        /// </summary>
        public PropertyCollectionImpl()
        {
        }

        /// <summary>
        /// Creates a new instance with the supplied items.
        /// </summary>
        public PropertyCollectionImpl(IEnumerable<Property> values) : base(values)
        {
        }
    }
}