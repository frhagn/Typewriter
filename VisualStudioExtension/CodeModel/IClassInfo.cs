using System;
using System.Collections.Generic;

namespace Typewriter.CodeModel
{
    public interface IClassInfo : IItemInfo
    {
        bool IsGeneric { get; }

        IEnumerable<IConstantInfo> Constants { get; }
        IEnumerable<IFieldInfo> Fields { get; }
        IEnumerable<IInterfaceInfo> Interfaces { get; }
        IEnumerable<IMethodInfo> Methods { get; }
        IEnumerable<IPropertyInfo> Properties { get; }
    }
}