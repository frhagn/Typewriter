using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.VisualStudio;
using VSLangProj;

namespace Typewriter.Generation.Controllers
{
    public static class SolutionExtensions
    {
        public static string Path(this ProjectItem projectItem)
        {
            try
            {
                return projectItem.Properties.Item("FullPath").Value.ToString();
            }
            catch (Exception e)
            {
                Log.Error($"Cannot find path of project item {e.Message}");
            }

            return null;
        }

        public static IEnumerable<Project> AllProjects(this Solution solution)
        {
            return GetProjects(solution.Projects);
        }

        public static IEnumerable<ProjectItem> AllProjectItems(this Project project, string extension)
        {
            if (project.ProjectItems == null)
                return Enumerable.Empty<ProjectItem>();

            return GetProjectItems(project.ProjectItems, extension);
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

                if (string.Equals(project.Kind, EnvDTE.Constants.vsProjectKindUnmodeled, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (string.Equals(project.Kind, ProjectKinds.vsProjectKindSolutionFolder, StringComparison.OrdinalIgnoreCase))
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

                if (string.Equals(subProject.Kind, EnvDTE.Constants.vsProjectKindUnmodeled, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // If this is another solution folder, do a recursive call, otherwise add
                if (string.Equals(subProject.Kind, ProjectKinds.vsProjectKindSolutionFolder, StringComparison.OrdinalIgnoreCase))
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

        private static IEnumerable<ProjectItem> GetProjectItems(ProjectItems projectItems, string extension)
        {
            foreach (ProjectItem item in projectItems)
            {
                if (item.Name.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase))
                {
                    yield return item;
                }

                if (item.ProjectItems != null && item.ProjectItems.Count>0)
                {
                    foreach (var subitem in GetProjectItems(item.ProjectItems, extension))
                    {
                        yield return subitem;
                    }
                }
            }
            
        }


        public static IEnumerable<ProjectItem> GetReferencedProjectItems(this VSProject vsproject, string extension)
        {
            
            foreach (Reference reference in vsproject.References)
            {
                var sp = reference.SourceProject;
                if (sp != null)
                {
                    foreach (var item in sp.AllProjectItems(extension))
                    {
                        yield return item;
                    }
                }
            }

            foreach (var item in vsproject.Project.AllProjectItems(extension))
            {
                yield return item;
            }
            
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T, int> action)
        {
            var ctr = 0;
            foreach (var item in items)
            {
                action(item, ctr);
                ctr++;
            }
        }

    }
}