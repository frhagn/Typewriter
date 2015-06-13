using System;
using System.Collections.Generic;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("File", "Files")]
    public interface File : Item
    {
        [Property("collection Classes", "All classes defined in the $context")]
        ICollection<Class> Classes { get; }

        [Property("collection Enums", "All enums defined in the $context")]
        ICollection<Enum> Enums { get; }

        [Property("collection Interfaces", "All interfaces defined in the $context")]
        ICollection<Interface> Interfaces { get; }
    }
}