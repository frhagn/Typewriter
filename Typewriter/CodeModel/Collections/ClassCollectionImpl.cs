using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class ClassCollectionImpl : List<Class>, ClassCollection
    {
        public ClassCollectionImpl()
        {
        }

        public ClassCollectionImpl(IEnumerable<Class> values) : base(values)
        {
        }
    }
}