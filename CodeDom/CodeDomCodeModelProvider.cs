using EnvDTE;
using Typewriter.CodeModel.Providers;

namespace Typewriter.CodeModel.CodeDom
{
    public class CodeDomCodeModelProvider : ICodeModelProvider
    {
        public File GetFile(ProjectItem projectItem)
        {
            return new CodeDomFile(projectItem);
        }
    }
}
