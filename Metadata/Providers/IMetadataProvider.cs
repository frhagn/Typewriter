using EnvDTE;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.Providers
{
    public interface IMetadataProvider
    {
        IFileMetadata GetFile(ProjectItem projectItem);
    }
}
