using System;
using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.CodeDom;

namespace Typewriter.Generation
{
    internal static class ItemFilter
    {
        internal static IEnumerable<object> Apply(IEnumerable<object> items, string filter, ref bool matchFound)
        {
            if (string.IsNullOrWhiteSpace(filter)) return items;

            Func<ItemInfo, IEnumerable<string>> selector;

            filter = filter.Trim();

            if (filter.StartsWith("[") && filter.EndsWith("]"))
            {
                filter = filter.Trim('[', ']', ' ');
                selector = i => i.Attributes.SelectMany(a => new[] { a.Name, a.FullName });
            }
            else if (filter.StartsWith(":"))
            {
                filter = filter.Remove(0, 1).Trim();
                selector = i => i.Interfaces.SelectMany(a => new[] { a.Name, a.FullName });
            }
            else
            {
                selector = i => new[] { i.Name, i.FullName };
            }

            var parts = filter.Split('*');
            var filtered = ApplyFilter(items, parts, selector);

            matchFound = matchFound || filtered.Any();

            return filtered;
        }

        private static ICollection<object> ApplyFilter(IEnumerable<object> items, string[] parts, Func<ItemInfo, IEnumerable<string>> selector)
        {
            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];

                if (parts.Length == 1)
                {
                    items = items.Cast<ItemInfo>().Where(item => selector(item).Any(p => p == part));
                }
                else if (i == 0 && string.IsNullOrWhiteSpace(part) == false)
                {
                    items = items.Cast<ItemInfo>().Where(item => selector(item).Any(p => p.StartsWith(part)));
                }
                else if (i == parts.Length - 1 && string.IsNullOrWhiteSpace(part) == false)
                {
                    items = items.Cast<ItemInfo>().Where(item => selector(item).Any(p => p.EndsWith(part)));
                }
                else if (i > 0 && i < parts.Length - 1 && string.IsNullOrWhiteSpace(part) == false)
                {
                    items = items.Cast<ItemInfo>().Where(item => selector(item).Any(p => p.Contains(part)));
                }
            }

            return items.ToList();
        }
    }
}
