using System;
using System.Collections.Generic;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Class", "Classes")]
    public interface Class : Item
    {
        [Property("string Namespace", "The namespace of the $context")]
        string Namespace { get; }

        [Property("bool IsGeneric", "Determines if the $context is generic")]
        bool IsGeneric { get; }

        [Property("collection Constants", "All constants defined in the $context")]
        ICollection<Constant> Constants { get; }

        [Property("collection Fields", "All fields defined in the $context")]
        ICollection<Field> Fields { get; }

        [Property("class BaseClass", "The base class of the $context")]
        Class BaseClass { get; }

        [Property("collection Interfaces", "All interfaces implemented by the $context")]
        ICollection<Interface> Interfaces { get; }

        [Property("collection Methods", "All methods defined in the $context")]
        ICollection<Method> Methods { get; }

        [Property("collection Properties", "All properties defined in the $context")]
        ICollection<Property> Properties { get; }

        [Property("collection GenericTypeArguments", "All generic type arguments of the $context")]
        IEnumerable<Type> GenericTypeArguments { get; }
    }
}