using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    /// <summary>
    /// Represents a collection of fields.
    /// </summary>
    public class FieldCollectionImpl : List<Field>, FieldCollection
    {
        /// <summary>
        /// Creates a new instance with no items.
        /// </summary>
        public FieldCollectionImpl()
        {
        }

        /// <summary>
        /// Creates a new instance with the supplied items.
        /// </summary>
        public FieldCollectionImpl(IEnumerable<Field> values) : base(values)
        {
        }
    }
}