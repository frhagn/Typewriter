using System;
using System.Collections.Generic;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Interface")]
    public interface IInterfaceInfo : IItemInfo
    {
        [Property("bool IsGeneric", "Determines if the $context is generic")]
        bool IsGeneric { get; }

        [Property("collection Interfaces", "All interfaces implemented by the $context")]
        ICollection<IInterfaceInfo> Interfaces { get; }

        [Property("collection Methods", "All methods defined in the $context")]
        ICollection<IMethodInfo> Methods { get; }

        [Property("collection Properties", "All properties defined in the $context")]
        ICollection<IPropertyInfo> Properties { get; }
    }
}