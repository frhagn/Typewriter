using Typewriter.Metadata.Interfaces;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class FileImpl : File
    {
        private readonly IFileMetadata metadata;

        public FileImpl(IFileMetadata metadata)
        {
            this.metadata = metadata;
        }

        public string Name => metadata.Name;
        public string FullName => metadata.FullName;

        private ClassCollection classes;
        public ClassCollection Classes => classes ?? (classes = ClassImpl.FromMetadata(metadata.Classes, this));

        private EnumCollection enums;
        public EnumCollection Enums => enums ?? (enums = EnumImpl.FromMetadata(metadata.Enums, this));

        private InterfaceCollection interfaces;
        public InterfaceCollection Interfaces => interfaces ?? (interfaces = InterfaceImpl.FromMetadata(metadata.Interfaces, this));
    }
}
