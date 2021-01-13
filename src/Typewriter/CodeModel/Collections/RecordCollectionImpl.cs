using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class RecordCollectionImpl : ItemCollectionImpl<Record>, RecordCollection
    {
        public RecordCollectionImpl(IEnumerable<Record> values) : base(values)
        {
        }

        protected override IEnumerable<string> GetAttributeFilter(Record item)
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

        protected override IEnumerable<string> GetInheritanceFilter(Record item)
        {
            if (item is null)
            {
                yield break;
            }

            if (item.BaseRecord != null)
            {
                yield return item.BaseRecord.Name;
                yield return item.BaseRecord.FullName;
            }

            foreach (var implementedInterface in item.Interfaces)
            {
                yield return implementedInterface.Name;
                yield return implementedInterface.FullName;
            }
        }

        protected override IEnumerable<string> GetItemFilter(Record item)
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