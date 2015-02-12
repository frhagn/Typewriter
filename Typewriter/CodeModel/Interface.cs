using System;
using System.Collections.Generic;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Interface", "Interfaces")]
    public interface Interface : Item
    {
        [Property("bool IsGeneric", "Determines if the $context is generic")]
        bool IsGeneric { get; }

        [Property("collection Interfaces", "All interfaces implemented by the $context")]
        ICollection<Interface> Interfaces { get; }

        [Property("collection Methods", "All methods defined in the $context")]
        ICollection<Method> Methods { get; }

        [Property("collection Properties", "All properties defined in the $context")]
        ICollection<Property> Properties { get; }
    }
}