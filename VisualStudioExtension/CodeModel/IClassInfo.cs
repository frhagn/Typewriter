using System;
using System.Collections.Generic;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Class")]
    public interface IClassInfo : IItemInfo
    {
        //[Property("Namespace", "The namespace of the $context")]
        //string Namespace { get; }

        [Property("bool IsGeneric", "Determines if the $context is generic")]
        bool IsGeneric { get; }

        [Property("collection Constants", "All constants defined in the $context")]
        ICollection<IConstantInfo> Constants { get; }

        [Property("collection Fields", "All fields defined in the $context")]
        ICollection<IFieldInfo> Fields { get; }

        [Property("collection Interfaces", "All interfaces implemented by the $context")]
        ICollection<IInterfaceInfo> Interfaces { get; }

        [Property("collection Methods", "All methods defined in the $context")]
        ICollection<IMethodInfo> Methods { get; }

        [Property("collection Properties", "All properties defined in the $context")]
        ICollection<IPropertyInfo> Properties { get; }
    }
}