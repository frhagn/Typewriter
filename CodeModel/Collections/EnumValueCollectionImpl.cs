using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    /// <summary>
    /// Represents a collection of enum values.
    /// </summary>
    public class EnumValueCollectionImpl : List<EnumValue>, EnumValueCollection
    {
        /// <summary>
        /// Creates a new instance with no items.
        /// </summary>
        public EnumValueCollectionImpl()
        {
        }

        /// <summary>
        /// Creates a new instance with the supplied items.
        /// </summary>
        public EnumValueCollectionImpl(IEnumerable<EnumValue> values) : base(values)
        {
        }
    }
}