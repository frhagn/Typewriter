using System;
using System.Collections.Generic;

namespace Typewriter.CodeModel
{
    public interface IMethodInfo : IItemInfo
    {
        IEnumerable<IParameterInfo> Parameters { get; }
        ITypeInfo Type { get; }
    }
}