using System;
using System.Collections.Generic;

namespace Typewriter.CodeModel
{
    public interface IMethodInfo : IItemInfo
    {
        ICollection<IParameterInfo> Parameters { get; }
        ITypeInfo Type { get; }
    }
}