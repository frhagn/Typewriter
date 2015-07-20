using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class ConstantCollectionImpl : ItemCollectionImpl<Constant>, ConstantCollection
    {
        public ConstantCollectionImpl(IEnumerable<Constant> values) : base(values)
        {
        }

        protected override IEnumerable<string> GetAttributeFilter(Constant item)
        {
            foreach (var attribute in item.Attributes)
            {
                yield return attribute.Name;
                yield return attribute.FullName;
            }
        }

        protected override IEnumerable<string> GetItemFilter(Constant item)
        {
            yield return item.Name;
            yield return item.FullName;
        }
    }
}