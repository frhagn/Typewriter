using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    /// <summary>
    /// Represents a collection of methods.
    /// </summary>
    public class MethodCollectionImpl : List<Method>, MethodCollection
    {
        /// <summary>
        /// Creates a new instance with no items.
        /// </summary>
        public MethodCollectionImpl()
        {
        }

        /// <summary>
        /// Creates a new instance with the supplied items.
        /// </summary>
        public MethodCollectionImpl(IEnumerable<Method> values) : base(values)
        {
        }
    }
}