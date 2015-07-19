using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class ParameterCollectionImpl : List<Parameter>, ParameterCollection
    {
        public ParameterCollectionImpl()
        {
        }

        public ParameterCollectionImpl(IEnumerable<Parameter> values) : base(values)
        {
        }
    }
}