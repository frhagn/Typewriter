using System.Collections.Generic;
using System.Linq;

namespace Typewriter.CodeModel.Collections
{
    public class TypeCollectionImpl : ItemCollectionImpl<Type>, TypeCollection
    {
        public TypeCollectionImpl(IEnumerable<Type> values) : base(values)
        {
        }

        protected override IEnumerable<string> GetAttributeFilter(Type item)
        {
            foreach (var attribute in item.Attributes)
            {
                yield return attribute.Name;
                yield return attribute.FullName;
            }
        }

        protected override IEnumerable<string> GetItemFilter(Type item)
        {
            yield return item.OriginalName;
            yield return item.FullName;
        }

        public override string ToString()
        {
            if(Count == 0)
                return string.Empty;

            return string.Concat("<", string.Join(", ", this.Select(t => t.Name)), ">");
        }
    }
}