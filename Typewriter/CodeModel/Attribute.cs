using System;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Attribute", "Attributes")]
    public interface Attribute : Item
    {
        [Property("string Value", "The value of the $context as string")]
        string Value { get; }
    }
}