using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class FieldCollectionImpl : List<Field>, FieldCollection
    {
        public FieldCollectionImpl()
        {
        }

        public FieldCollectionImpl(IEnumerable<Field> values) : base(values)
        {
        }
    }
}