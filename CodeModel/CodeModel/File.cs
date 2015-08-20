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
        /// All classes defined in the file.
        /// </summary>
        public abstract ClassCollection Classes { get; }

        /// <summary>
        /// All enums defined in the file.
        /// </summary>
        public abstract EnumCollection Enums { get; }

        /// <summary>
        /// All interfaces defined in the file.
        /// </summary>
        public abstract InterfaceCollection Interfaces { get; }
        
        /// <summary>
        /// The full path of the file.
        /// </summary>
        public abstract string FullName { get; }

        /// <summary>
        /// The name of the file.
        /// </summary>
        public abstract string Name { get; }
    }
}