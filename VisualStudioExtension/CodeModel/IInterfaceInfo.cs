using System;
using System.Collections.Generic;

namespace Typewriter.CodeModel
{
    public interface IInterfaceInfo : IItemInfo
    {
        bool IsGeneric { get; }

        IEnumerable<IInterfaceInfo> Interfaces { get; }
        IEnumerable<IMethodInfo> Methods { get; }
        IEnumerable<IPropertyInfo> Properties { get; }
    }
}