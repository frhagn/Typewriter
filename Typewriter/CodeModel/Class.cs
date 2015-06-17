using System;
using System.Collections.Generic;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Class", "Classes")]
    public interface Class : CodeItem
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
        ICollection<Type> GenericTypeArguments { get; }

        [Property("collection NestedClasses", "All classes defined in the $context")]
        ICollection<Class> NestedClasses { get; }

        [Property("collection NestedEnums", "All enums defined in the $context")]
        ICollection<Enum> NestedEnums { get; }

        [Property("collection NestedInterfaces", "All interfaces defined in the $context")]
        ICollection<Interface> NestedInterfaces { get; }
    }
}