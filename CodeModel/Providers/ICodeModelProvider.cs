using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace Typewriter.CodeModel.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICodeModelProvider
    {
        /// <summary>
        /// 
        /// </summary>
        File GetFile(ProjectItem projectItem);
    }
}
