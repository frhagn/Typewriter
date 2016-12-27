using System;
using Typewriter.Configuration;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.Providers
{
    public interface IMetadataProvider
    {
        IFileMetadata GetFile(string path, Settings settings, Action<string[]> requestRender);
    }
}
