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
        /// All public classes defined in the file.
        /// </summary>
        public abstract ClassCollection Classes { get; }

        /// <summary>
        /// All public delegates defined in the file.
        /// </summary>
        public abstract DelegateCollection Delegates { get; }

        /// <summary>
        /// All public enums defined in the file.
        /// </summary>
        public abstract EnumCollection Enums { get; }

        /// <summary>
        /// All public interfaces defined in the file.
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