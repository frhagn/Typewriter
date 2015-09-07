using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.Providers
{
    public interface IMetadataProvider
    {
        IFileMetadata GetFile(string path);
    }
}
