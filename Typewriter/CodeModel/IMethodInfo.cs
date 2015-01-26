using System;
using System.Collections.Generic;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Method", "Methods")]
    public interface IMethodInfo : IFieldInfo
    {
        [Property("collection Parameters", "All parameters defined in the $context")]
        ICollection<IParameterInfo> Parameters { get; }
    }
}