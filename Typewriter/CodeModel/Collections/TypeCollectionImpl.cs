using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class TypeCollectionImpl : List<Type>, TypeCollection
    {
        public TypeCollectionImpl()
        {
        }

        public TypeCollectionImpl(IEnumerable<Type> values) : base(values)
        {
        }
    }
}