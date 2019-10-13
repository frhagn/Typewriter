using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class AttributeArgumentCollectionImpl : ItemCollectionImpl<AttributeArgument>, AttributeArgumentCollection
    {
        public AttributeArgumentCollectionImpl(IEnumerable<AttributeArgument> values) : base(values)
        {
        }
    }
}