using System;
using System.Collections.Generic;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("EnumValue", "Values")]
    public interface EnumValue : CodeItem
    {
        [Property("number Value", "The numeric value of the $context")]
        int Value { get; }
    }
}