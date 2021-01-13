﻿using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class FieldCollectionImpl : ItemCollectionImpl<Field>, FieldCollection
    {
        public FieldCollectionImpl(IEnumerable<Field> values) : base(values)
        {
        }

        protected override IEnumerable<string> GetAttributeFilter(Field item)
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

        protected override IEnumerable<string> GetItemFilter(Field item)
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