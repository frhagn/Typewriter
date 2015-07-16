using EnvDTE;
using Typewriter.CodeModel.Providers;

namespace Typewriter.CodeModel.CodeDom
{
    public class CodeDomProvider : ICodeModelProvider
    {
        public File GetFile(ProjectItem projectItem)
        {
            return new CodeDomFile(projectItem);
        }
    }
}
