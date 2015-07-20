using EnvDTE;
using Typewriter.Metadata.Interfaces;
using Typewriter.Metadata.Providers;

namespace Typewriter.Metadata.CodeDom
{
    public class CodeDomMetadataProvider : IMetadataProvider
    {
        public IFileMetadata GetFile(ProjectItem projectItem)
        {
            return new CodeDomFileMetadata(projectItem);
        }
    }
}
