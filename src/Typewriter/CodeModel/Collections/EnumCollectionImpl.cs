using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class EnumCollectionImpl : ItemCollectionImpl<Enum>, EnumCollection
    {
        public EnumCollectionImpl(IEnumerable<Enum> values) : base(values)
        {
        }

        protected override IEnumerable<string> GetAttributeFilter(Enum item)
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

        protected override IEnumerable<string> GetItemFilter(Enum item)
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