using System;
using System.Collections.Generic;

namespace Typewriter.CodeModel.Collections
{
    public abstract class ItemCollectionImpl<T> : List<T>, ItemCollection<T> where T : Item
    {
        protected ItemCollectionImpl(IEnumerable<T> values) : base(values)
        {
        }

        public Func<Item, IEnumerable<string>> AttributeFilterSelector => i =>
        {
            return !(i is T item) ? Array.Empty<string>() : GetAttributeFilter(item);
        };

        public Func<Item, IEnumerable<string>> InheritanceFilterSelector => i =>
        {
            return !(i is T item) ? Array.Empty<string>() : GetInheritanceFilter(item);
        };

        public Func<Item, IEnumerable<string>> ItemFilterSelector => i =>
        {
            return !(i is T item) ? Array.Empty<string>() : GetItemFilter(item);
        };

        protected virtual IEnumerable<string> GetAttributeFilter(T item)
        {
            return Array.Empty<string>();
        }

        protected virtual IEnumerable<string> GetInheritanceFilter(T item)
        {
            return Array.Empty<string>();
        }

        protected virtual IEnumerable<string> GetItemFilter(T item)
        {
            return Array.Empty<string>();
        }
    }
}