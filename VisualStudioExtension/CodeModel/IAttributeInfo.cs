using System;

namespace Typewriter.CodeModel
{
    public interface IAttributeInfo : IItemInfo
    {
        string Value { get; }
    }
}