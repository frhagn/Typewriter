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
            var item = i as T;
            return item == null ? new string[0] : GetAttributeFilter(item);
        };

        public Func<Item, IEnumerable<string>> InheritanceFilterSelector => i =>
        {
            var item = i as T;
            return item == null ? new string[0] : GetInheritanceFilter(item);
        };

        public Func<Item, IEnumerable<string>> ItemFilterSelector => i =>
        {
            var item = i as T;
            return item == null ? new string[0] : GetItemFilter(item);
        };

        protected virtual IEnumerable<string> GetAttributeFilter(T item)
        {
            return new string[0];
        }

        protected virtual IEnumerable<string> GetInheritanceFilter(T item)
        {
            return new string[0];
        }

        protected virtual IEnumerable<string> GetItemFilter(T item)
        {
            return new string[0];
        }
    }
}