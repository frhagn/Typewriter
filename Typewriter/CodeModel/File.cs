using System;
using System.Collections.Generic;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("File", "Files")]
    public interface File
    {
        [Property("string Name", "The name of the $context")]
        string Name { get; }

        [Property("string Full name", "The full name of the $context")]
        string FullName { get; }

        [Property("collection Classes", "All classes defined in the $context")]
        ICollection<Class> Classes { get; }

        [Property("collection Enums", "All enums defined in the $context")]
        ICollection<Enum> Enums { get; }

        [Property("collection Interfaces", "All interfaces defined in the $context")]
        ICollection<Interface> Interfaces { get; }
    }
}