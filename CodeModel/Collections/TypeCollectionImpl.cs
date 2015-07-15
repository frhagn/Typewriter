using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    /// <summary>
    /// Represents a collection of types.
    /// </summary>
    public class TypeCollectionImpl : List<Type>, TypeCollection
    {
        /// <summary>
        /// Creates a new instance with no items.
        /// </summary>
        public TypeCollectionImpl()
        {
        }

        /// <summary>
        /// Creates a new instance with the supplied items.
        /// </summary>
        public TypeCollectionImpl(IEnumerable<Type> values) : base(values)
        {
        }
    }
}