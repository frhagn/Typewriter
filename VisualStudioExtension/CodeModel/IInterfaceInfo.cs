using System;
using System.Collections.Generic;

namespace Typewriter.CodeModel
{
    public interface IInterfaceInfo : IItemInfo
    {
        bool IsGeneric { get; }

        ICollection<IInterfaceInfo> Interfaces { get; }
        ICollection<IMethodInfo> Methods { get; }
        ICollection<IPropertyInfo> Properties { get; }
    }
}