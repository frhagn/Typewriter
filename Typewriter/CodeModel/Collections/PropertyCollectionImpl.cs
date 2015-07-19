using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class PropertyCollectionImpl : List<Property>, PropertyCollection
    {
        public PropertyCollectionImpl()
        {
        }

        public PropertyCollectionImpl(IEnumerable<Property> values) : base(values)
        {
        }
    }
}