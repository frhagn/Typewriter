using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    /// <summary>
    /// Represents a collection of attributes.
    /// </summary>
    public class AttributeCollectionImpl : List<Attribute>, AttributeCollection
    {
        /// <summary>
        /// Creates a new instance with no items.
        /// </summary>
        public AttributeCollectionImpl()
        {
        }

        /// <summary>
        /// Creates a new instance with the supplied items.
        /// </summary>
        public AttributeCollectionImpl(IEnumerable<Attribute> values) : base(values)
        {
        }
    }
}