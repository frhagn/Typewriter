using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class PropertyCollectionImpl : ItemCollectionImpl<Property>, PropertyCollection
    {
        public PropertyCollectionImpl(IEnumerable<Property> values) : base(values)
        {
        }

        protected override IEnumerable<string> GetAttributeFilter(Property item)
        {
            foreach (var attribute in item.Attributes)
            {
                yield return attribute.Name;
                yield return attribute.FullName;
            }
        }

        protected override IEnumerable<string> GetItemFilter(Property item)
        {
            yield return item.Name;
            yield return item.FullName;
        }
    }
}