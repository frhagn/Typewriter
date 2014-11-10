using System;
using System.Collections.Generic;

namespace Typewriter.CodeModel
{
    public interface IEnumInfo : IItemInfo
    {
        IEnumerable<IEnumValueInfo> Values { get; }
    }
}