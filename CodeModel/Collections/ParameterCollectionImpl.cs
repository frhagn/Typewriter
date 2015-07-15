using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    /// <summary>
    /// Represents a collection of parameters.
    /// </summary>
    public class ParameterCollectionImpl : List<Parameter>, ParameterCollection
    {
        /// <summary>
        /// Creates a new instance with no items.
        /// </summary>
        public ParameterCollectionImpl()
        {
        }

        /// <summary>
        /// Creates a new instance with the supplied items.
        /// </summary>
        public ParameterCollectionImpl(IEnumerable<Parameter> values) : base(values)
        {
        }
    }
}