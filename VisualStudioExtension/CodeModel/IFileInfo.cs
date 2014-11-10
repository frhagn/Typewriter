using System;
using System.Collections.Generic;

namespace Typewriter.CodeModel
{
    public interface IFileInfo
    {
        string Name { get; }
        string FullName { get; }

        IEnumerable<IClassInfo> Classes { get; }
        IEnumerable<IEnumInfo> Enums { get; }
        IEnumerable<IInterfaceInfo> Interfaces { get; }
    }
}