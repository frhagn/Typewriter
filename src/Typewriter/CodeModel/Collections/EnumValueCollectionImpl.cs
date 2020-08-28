using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class EnumValueCollectionImpl : ItemCollectionImpl<EnumValue>, EnumValueCollection
    {
        public EnumValueCollectionImpl(IEnumerable<EnumValue> values) : base(values)
        {
        }

        protected override IEnumerable<string> GetAttributeFilter(EnumValue item)
        {
            if (item is null)
            {
                yield break;
            }

            foreach (var attribute in item.Attributes)
            {
                yield return attribute.Name;
                yield return attribute.FullName;
            }
        }

        protected override IEnumerable<string> GetItemFilter(EnumValue item)
        {
            if (item is null)
            {
                yield break;
            }

            yield return item.Name;
            yield return item.FullName;
        }
    }
}