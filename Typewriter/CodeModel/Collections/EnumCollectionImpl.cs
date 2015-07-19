using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class EnumCollectionImpl : List<Enum>, EnumCollection
    {
        public EnumCollectionImpl()
        {
        }

        public EnumCollectionImpl(IEnumerable<Enum> values) : base(values)
        {
        }
    }
}