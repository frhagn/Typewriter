using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    /// <summary>
    /// Represents a collection of interfaces.
    /// </summary>
    public class InterfaceCollectionImpl : List<Interface>, InterfaceCollection
    {
        /// <summary>
        /// Creates a new instance with no items.
        /// </summary>
        public InterfaceCollectionImpl()
        {
        }

        /// <summary>
        /// Creates a new instance with the supplied items.
        /// </summary>
        public InterfaceCollectionImpl(IEnumerable<Interface> values) : base(values)
        {
        }
    }
}