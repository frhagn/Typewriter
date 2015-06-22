using System.Collections.Generic;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Method", "Methods")]
    public interface Method : Field
    {
        [Property("bool IsGeneric", "Determines if the $context is generic")]
        bool IsGeneric { get; }

        [Property("collection GenericTypeArguments", "All generic type arguments of the $context")]
        ICollection<Type> GenericTypeArguments { get; }

        [Property("collection Parameters", "All parameters defined in the $context")]
        ICollection<Parameter> Parameters { get; }
    }
}