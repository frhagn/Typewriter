using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    /// <summary>
    /// Represents a collection of constants.
    /// </summary>
    public class ConstantCollectionImpl : List<Constant>, ConstantCollection
    {
        /// <summary>
        /// Creates a new instance with no items.
        /// </summary>
        public ConstantCollectionImpl()
        {
        }

        /// <summary>
        /// Creates a new instance with the supplied items.
        /// </summary>
        public ConstantCollectionImpl(IEnumerable<Constant> values) : base(values)
        {
        }
    }
}