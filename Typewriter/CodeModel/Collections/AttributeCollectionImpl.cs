using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class AttributeCollectionImpl : ItemCollectionImpl<Attribute>, AttributeCollection
    {
        public AttributeCollectionImpl(IEnumerable<Attribute> values) : base(values)
        {
        }

        protected override IEnumerable<string> GetItemFilter(Attribute item)
        {
            yield return item.Name;
            yield return item.FullName;
        }
    }
}