using System;
using System.Collections.Generic;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Method")]
    public interface IMethodInfo : IItemInfo
    {
        [Property("collection Parameters", "All parameters defined in the $context")]
        ICollection<IParameterInfo> Parameters { get; }

        [Property("type Type", "The type of the $context")]
        ITypeInfo Type { get; }
    }
}