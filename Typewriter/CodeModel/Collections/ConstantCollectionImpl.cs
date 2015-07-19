using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class ConstantCollectionImpl : List<Constant>, ConstantCollection
    {
        public ConstantCollectionImpl()
        {
        }

        public ConstantCollectionImpl(IEnumerable<Constant> values) : base(values)
        {
        }
    }
}