using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class MethodCollectionImpl : List<Method>, MethodCollection
    {
        public MethodCollectionImpl()
        {
        }

        public MethodCollectionImpl(IEnumerable<Method> values) : base(values)
        {
        }
    }
}