using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    /// <summary>
    /// Represents a collection of enums.
    /// </summary>
    public class EnumCollectionImpl : List<Enum>, EnumCollection
    {
        /// <summary>
        /// Creates a new instance with no items.
        /// </summary>
        public EnumCollectionImpl()
        {
        }

        /// <summary>
        /// Creates a new instance with the supplied items.
        /// </summary>
        public EnumCollectionImpl(IEnumerable<Enum> values) : base(values)
        {
        }
    }
}