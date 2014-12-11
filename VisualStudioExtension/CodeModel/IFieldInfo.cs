using System;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Field")]
    public interface IFieldInfo : IItemInfo
    {
        [Property("type Type", "The type of the $context")]
        ITypeInfo Type { get; }
    }
}