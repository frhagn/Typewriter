using System;
using System.Collections.Generic;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Interface", "Interfaces")]
    public interface Interface : CodeItem
    {
        [Property("string Namespace", "The namespace of the $context")]
        string Namespace { get; }

        [Property("bool IsGeneric", "Determines if the $context is generic")]
        bool IsGeneric { get; }

        [Property("collection GenericTypeArguments", "All generic type arguments of the $context")]
        ICollection<Type> GenericTypeArguments { get; }

        [Property("collection Interfaces", "All interfaces implemented by the $context")]
        ICollection<Interface> Interfaces { get; }

        [Property("collection Methods", "All methods defined in the $context")]
        ICollection<Method> Methods { get; }

        [Property("collection Properties", "All properties defined in the $context")]
        ICollection<Property> Properties { get; }

        [Property("class ContainingClass", "The containing class of the $context if it is nested")]
        Class ContainingClass { get; }
    }
}