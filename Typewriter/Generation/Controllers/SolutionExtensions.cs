using System;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;

namespace Typewriter.Generation.Controllers
{
    public static class SolutionExtensions
    {
        public static IEnumerable<Project> AllProjetcs(this Solution solution)
        {
            return GetProjects(solution.Projects);
        }

        public static IEnumerable<ProjectItem> AllProjectItems(this Project project)
        {
            if (project.ProjectItems == null)
                return new List<ProjectItem>();

            return GetProjectItems(project.ProjectItems);
        }

        private static IEnumerable<Project> GetProjects(Projects projects)
        {
            var list = new List<Project>();
            var item = projects.GetEnumerator();
            while (item.MoveNext())
            {
                var project = item.Current as Project;
                if (project == null)
                {
                    continue;
                }

                if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    list.AddRange(GetSolutionFolderProjects(project));
                }
                else
                {
                    list.Add(project);
                }
            }

            return list;
        }

        private static IEnumerable<Project> GetSolutionFolderProjects(Project solutionFolder)
        {
            var list = new List<Project>();

            for (var i = 1; i <= solutionFolder.ProjectItems.Count; i++)
            {
                var subProject = solutionFolder.ProjectItems.Item(i).SubProject;
                if (subProject == null)
                {
                    continue;
                }

                // If this is another solution folder, do a recursive call, otherwise add
                if (subProject.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    list.AddRange(GetSolutionFolderProjects(subProject));
                }
                else
                {
                    list.Add(subProject);
                }
            }

            return list;
        }

        private static IEnumerable<ProjectItem> GetProjectItems(ProjectItems projectItems)
        {
            var items = new List<ProjectItem>();

            foreach (ProjectItem item in projectItems)
            {
                items.Add(item);

                if (item.ProjectItems != null)
                {
                    items.AddRange(GetProjectItems(item.ProjectItems));
                }
            }

            return items;
        }
    }
}