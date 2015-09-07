using System;
using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel;

namespace Typewriter.Generation
{
    internal static class ItemFilter
    {
        internal static IEnumerable<Item> Apply(IEnumerable<Item> items, string filter, ref bool matchFound)
        {
            if (string.IsNullOrWhiteSpace(filter)) return items;

            var filterable = items as IFilterable;
            if (filterable == null) return items;

            Func<Item, IEnumerable<string>> selector;

            filter = filter.Trim();

            if (filter.StartsWith("[") && filter.EndsWith("]"))
            {
                filter = filter.Trim('[', ']', ' ');
                selector = filterable.AttributeFilterSelector;
            }
            else if (filter.StartsWith(":"))
            {
                filter = filter.Remove(0, 1).Trim();
                selector = filterable.InheritanceFilterSelector;
            }
            else
            {
                selector = filterable.ItemFilterSelector;
            }

            var filtered = ApplyFilter(items, filter, selector);

            matchFound = matchFound || filtered.Any();

            return filtered;
        }

        private static ICollection<Item> ApplyFilter(IEnumerable<Item> items, string filter, Func<Item, IEnumerable<string>> selector)
        {
            var parts = filter.Split('*');

            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];

                if (parts.Length == 1)
                {
                    items = items.Where(item => selector(item).Any(p => p == part));
                }
                else if (i == 0 && string.IsNullOrWhiteSpace(part) == false)
                {
                    items = items.Where(item => selector(item).Any(p => p.StartsWith(part)));
                }
                else if (i == parts.Length - 1 && string.IsNullOrWhiteSpace(part) == false)
                {
                    items = items.Where(item => selector(item).Any(p => p.EndsWith(part)));
                }
                else if (i > 0 && i < parts.Length - 1 && string.IsNullOrWhiteSpace(part) == false)
                {
                    items = items.Where(item => selector(item).Any(p => p.Contains(part)));
                }
            }

            return items.ToList();
        }
    }
}
