using System;
using System.Collections.Generic;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Enum", "Enums")]
    public interface Enum : Item
    {
        [Property("collection Values", "All values defined in the $context")]
        ICollection<EnumValue> Values { get; }
    }
}