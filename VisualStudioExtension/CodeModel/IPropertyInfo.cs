using System;

namespace Typewriter.CodeModel
{
    public interface IPropertyInfo : IFieldInfo
    {
        bool HasGetter { get; }
        bool HasSetter { get; }
    }
}