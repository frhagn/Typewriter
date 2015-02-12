using System;
using System.Collections.Generic;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Method", "Methods")]
    public interface Method : Field
    {
        [Property("collection Parameters", "All parameters defined in the $context")]
        ICollection<Parameter> Parameters { get; }
    }
}