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

        public override string Name => metadata.Name;
        public override string FullName => metadata.FullName;

        private ClassCollection classes;
        public override ClassCollection Classes => classes ?? (classes = ClassImpl.FromMetadata(metadata.Classes, this));

        private EnumCollection enums;
        public override EnumCollection Enums => enums ?? (enums = EnumImpl.FromMetadata(metadata.Enums, this));

        private InterfaceCollection interfaces;
        public override InterfaceCollection Interfaces => interfaces ?? (interfaces = InterfaceImpl.FromMetadata(metadata.Interfaces, this));

        public override string ToString()
        {
            return Name;
        }
    }
}
