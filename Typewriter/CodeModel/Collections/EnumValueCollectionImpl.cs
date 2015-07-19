using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class EnumValueCollectionImpl : List<EnumValue>, EnumValueCollection
    {
        public EnumValueCollectionImpl()
        {
        }

        public EnumValueCollectionImpl(IEnumerable<EnumValue> values) : base(values)
        {
        }
    }
}