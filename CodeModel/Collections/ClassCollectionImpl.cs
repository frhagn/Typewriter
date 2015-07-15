using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    /// <summary>
    /// Represents a collection of classes.
    /// </summary>
    public class ClassCollectionImpl : List<Class>, ClassCollection
    {
        /// <summary>
        /// Creates a new instance with no items.
        /// </summary>
        public ClassCollectionImpl()
        {
        }

        /// <summary>
        /// Creates a new instance with the supplied items.
        /// </summary>
        public ClassCollectionImpl(IEnumerable<Class> values) : base(values)
        {
        }
    }
}