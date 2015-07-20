using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class ClassCollectionImpl : ItemCollectionImpl<Class>, ClassCollection
    {
        public ClassCollectionImpl(IEnumerable<Class> values) : base(values)
        {
        }

        protected override IEnumerable<string> GetAttributeFilter(Class item)
        {
            foreach (var attribute in item.Attributes)
            {
                yield return attribute.Name;
                yield return attribute.FullName;
            }
        }

        protected override IEnumerable<string> GetInheritanceFilter(Class item)
        {
            if (item.BaseClass != null)
            {
                yield return item.BaseClass.Name;
                yield return item.BaseClass.FullName;
            }

            foreach (var implementedInterface in item.Interfaces)
            {
                yield return implementedInterface.Name;
                yield return implementedInterface.FullName;
            }
        }

        protected override IEnumerable<string> GetItemFilter(Class item)
        {
            yield return item.Name;
            yield return item.FullName;
        }
    }
}