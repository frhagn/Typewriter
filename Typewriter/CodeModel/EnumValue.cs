using System;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("EnumValue", "Values")]
    public interface EnumValue : Item
    {
        [Property("number Value", "The numeric value of the $context")]
        int Value { get; }
    }
}