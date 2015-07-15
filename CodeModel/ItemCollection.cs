using System.Collections.Generic;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a collection of items.
    /// </summary>
    public interface ItemCollection<out T> : IReadOnlyList<T>
    {
    }
}