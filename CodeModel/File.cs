using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a file.
    /// </summary>
    [Context("File", "Files")]
    public interface File : Item
    {
        /// <summary>
        /// All classes defined in the file.
        /// </summary>
        [Property("collection Classes", "All classes defined in the $context")]
        ClassCollection Classes { get; }

        /// <summary>
        /// All enums defined in the file.
        /// </summary>
        [Property("collection Enums", "All enums defined in the $context")]
        EnumCollection Enums { get; }

        /// <summary>
        /// All interfaces defined in the file.
        /// </summary>
        [Property("collection Interfaces", "All interfaces defined in the $context")]
        InterfaceCollection Interfaces { get; }
    }
}