using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class MethodCollectionImpl : ItemCollectionImpl<Method>, MethodCollection
    {
        public MethodCollectionImpl(IEnumerable<Method> values) : base(values)
        {
        }

        protected override IEnumerable<string> GetAttributeFilter(Method item)
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

        protected override IEnumerable<string> GetItemFilter(Method item)
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