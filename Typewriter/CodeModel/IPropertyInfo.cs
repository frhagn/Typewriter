using System;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Property")]
    public interface IPropertyInfo : IFieldInfo
    {
        [Property("bool HasGetter", "Determines if the $context has a getter")]
        bool HasGetter { get; }

        [Property("bool HasSetter", "Determines if the $context has a setter")]
        bool HasSetter { get; }
    }
}