using System;
using System.Collections.Generic;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Attribute", "Attributes")]
    public interface Attribute : CodeItem
    {
        [Property("string Value", "The value of the $context as string")]
        string Value { get; }
    }
}