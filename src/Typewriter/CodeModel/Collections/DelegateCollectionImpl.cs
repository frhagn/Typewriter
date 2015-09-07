using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class DelegateCollectionImpl : ItemCollectionImpl<Delegate>, DelegateCollection
    {
        public DelegateCollectionImpl(IEnumerable<Delegate> values) : base(values)
        {
        }

        protected override IEnumerable<string> GetAttributeFilter(Delegate item)
        {
            foreach (var attribute in item.Attributes)
            {
                yield return attribute.Name;
                yield return attribute.FullName;
            }
        }

        protected override IEnumerable<string> GetItemFilter(Delegate item)
        {
            yield return item.Name;
            yield return item.FullName;
        }
    }
}