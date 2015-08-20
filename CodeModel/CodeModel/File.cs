using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a file.
    /// </summary>
    [Context("File", "Files")]
    public abstract class File : Item
    {
        /// <summary>
        /// The name of the file.
        /// </summary>
        //[Property("string Name", "The name of the $context.")]
        public abstract string Name { get; }

        /// <summary>
        /// The full path of the file.
        /// </summary>
        //[Property("string FullName", "The full path of the $context.")]
        public abstract string FullName { get; }

        /// <summary>
        /// All classes defined in the file.
        /// </summary>
        //[Property("collection Classes", "All classes defined in the $context.")]
        public abstract ClassCollection Classes { get; }

        /// <summary>
        /// All enums defined in the file.
        /// </summary>
        //[Property("collection Enums", "All enums defined in the $context.")]
        public abstract EnumCollection Enums { get; }

        /// <summary>
        /// All interfaces defined in the file.
        /// </summary>
        //[Property("collection Interfaces", "All interfaces defined in the $context.")]
        public abstract InterfaceCollection Interfaces { get; }
    }
}