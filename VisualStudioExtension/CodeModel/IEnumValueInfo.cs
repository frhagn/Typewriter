using System;

namespace Typewriter.CodeModel
{
    public interface IEnumValueInfo : IItemInfo
    {
        int Value { get; }
    }
}