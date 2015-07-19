using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class InterfaceCollectionImpl : List<Interface>, InterfaceCollection
    {
        public InterfaceCollectionImpl()
        {
        }

        public InterfaceCollectionImpl(IEnumerable<Interface> values) : base(values)
        {
        }
    }
}