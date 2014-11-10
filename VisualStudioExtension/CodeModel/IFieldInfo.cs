using System;

namespace Typewriter.CodeModel
{
    public interface IFieldInfo : IItemInfo
    {
        ITypeInfo Type { get; }
    }
}