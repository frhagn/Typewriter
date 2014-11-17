using System;
using System.Collections.Generic;

namespace Typewriter.CodeModel
{
    public interface IClassInfo : IItemInfo
    {
        bool IsGeneric { get; }

        ICollection<IConstantInfo> Constants { get; }
        ICollection<IFieldInfo> Fields { get; }
        ICollection<IInterfaceInfo> Interfaces { get; }
        ICollection<IMethodInfo> Methods { get; }
        ICollection<IPropertyInfo> Properties { get; }
    }
}