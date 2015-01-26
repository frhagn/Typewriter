using System;
using System.Collections.Generic;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Enum", "Enums")]
    public interface IEnumInfo : IItemInfo
    {
        [Property("collection Values", "All values defined in the $context")]
        ICollection<IEnumValueInfo> Values { get; }
    }
}