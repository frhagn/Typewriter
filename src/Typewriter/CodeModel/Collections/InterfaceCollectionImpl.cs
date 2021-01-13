using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class InterfaceCollectionImpl : ItemCollectionImpl<Interface>, InterfaceCollection
    {
        public InterfaceCollectionImpl(IEnumerable<Interface> values) : base(values)
        {
        }

        protected override IEnumerable<string> GetAttributeFilter(Interface item)
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

        protected override IEnumerable<string> GetInheritanceFilter(Interface item)
        {
            if (item is null)
            {
                yield break;
            }

            foreach (var implementedInterface in item.Interfaces)
            {
                yield return implementedInterface.Name;
                yield return implementedInterface.FullName;
            }
        }

        protected override IEnumerable<string> GetItemFilter(Interface item)
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