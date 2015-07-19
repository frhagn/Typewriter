using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class AttributeCollectionImpl : List<Attribute>, AttributeCollection
    {
        public AttributeCollectionImpl()
        {
        }

        public AttributeCollectionImpl(IEnumerable<Attribute> values) : base(values)
        {
        }
    }
}