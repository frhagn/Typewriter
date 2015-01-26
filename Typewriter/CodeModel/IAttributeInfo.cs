using System;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Attribute", "Attributes")]
    public interface IAttributeInfo : IItemInfo
    {
        [Property("string Value", "The value of the $context as string")]
        string Value { get; }
    }
}