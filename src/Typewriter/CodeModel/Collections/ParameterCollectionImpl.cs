﻿using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public class ParameterCollectionImpl : ItemCollectionImpl<Parameter>, ParameterCollection
    {
        public ParameterCollectionImpl(IEnumerable<Parameter> values) : base(values)
        {
        }

        protected override IEnumerable<string> GetAttributeFilter(Parameter item)
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

        protected override IEnumerable<string> GetItemFilter(Parameter item)
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