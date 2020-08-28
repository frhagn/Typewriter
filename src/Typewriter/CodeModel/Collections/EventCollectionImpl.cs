using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class EventCollectionImpl : ItemCollectionImpl<Event>, EventCollection
    {
        public EventCollectionImpl(IEnumerable<Event> values) : base(values)
        {
        }

        protected override IEnumerable<string> GetAttributeFilter(Event item)
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

        protected override IEnumerable<string> GetItemFilter(Event item)
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