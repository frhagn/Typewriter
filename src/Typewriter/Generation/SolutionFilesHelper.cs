using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Typewriter.Generation.Controllers;

namespace Typewriter.Generation
{
    public class SolutionFilesHelper
    {
        private DTE dte;

        public SolutionFilesHelper()
        {
            dte = (DTE)Package.GetGlobalService(typeof(DTE));
        }

        public IEnumerable<ProjectItem> Recurse(ProjectItems i)
        {
            if (i != null)
            {
                foreach (ProjectItem j in i)
                {
                    foreach (ProjectItem k in Recurse(j))
                    {
                        yield return k;
                    }
                }

            }
        }
        public IEnumerable<ProjectItem> Recurse(ProjectItem i)
        {
            yield return i;
            foreach (ProjectItem j in Recurse(i.ProjectItems))
            {
                yield return j;
            }
        }

        public IEnumerable<ProjectItem> SolutionFiles()
        {
            foreach (var project in dte.Solution.AllProjects())
            {
                foreach (ProjectItem item in Recurse(project.ProjectItems))
                {
                    yield return item;
                }
            }
        }
    }
}
