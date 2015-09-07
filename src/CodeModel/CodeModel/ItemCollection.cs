using System;
using System.Collections.Generic;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a collection of items.
    /// </summary>
    public interface ItemCollection<out T> : IReadOnlyList<T>, IFilterable
    {
    }

    /// <summary>
    /// Provides access to filter selectors for item collections.
    /// </summary>
    public interface IFilterable
    {
        /// <summary>
        /// Returns a selector to filter items by attributes.
        /// </summary>
        Func<Item, IEnumerable<string>> AttributeFilterSelector { get; }

        /// <summary>
        /// Returns a selector to filter items by inheritance.
        /// </summary>
        Func<Item, IEnumerable<string>> InheritanceFilterSelector { get; }

        /// <summary>
        /// Returns a selector to filter items by name.
        /// </summary>
        Func<Item, IEnumerable<string>> ItemFilterSelector { get; }
    }
}