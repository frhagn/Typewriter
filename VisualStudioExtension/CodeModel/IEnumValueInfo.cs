using System;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Enum Value")]
    public interface IEnumValueInfo : IItemInfo
    {
        [Property("number Value", "The numeric value of the $context")]
        int Value { get; }
    }
}