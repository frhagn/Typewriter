using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using Typewriter.Generation.Controllers;
using Typewriter.VisualStudio;
using VSLangProj;

namespace Typewriter.CodeModel.Configuration
{
    internal static class ProjectHelpers
    {
        internal static void AddProject(ProjectItem projectItem, ICollection<string> projectList, string projectName)
        {
            foreach (var project in projectItem.DTE.Solution.AllProjects())
            {
                try
                {
                    if (project.Name == projectName)
                    {
                        AddProject(projectList, project);
                        return;
                    }
                }
                catch (Exception exception)
                {
                    Log.Debug($"Cannot add project named '{projectName}' ({exception.Message})");
                }
            }

            string message = $"Cannot find project named '{projectName}'";

            ErrorList.AddWarning(projectItem, message);
            Log.Warn(message);
        }

        internal static void AddCurrentProject(ICollection<string> projectList, ProjectItem projectItem)
        {
            var vsproject = projectItem.ContainingProject.Object as VSProject;
            if (vsproject != null)
            {
                AddProject(projectList, projectItem.ContainingProject);
            }
        }

        internal static void AddReferencedProjects(ICollection<string> projectList, ProjectItem projectItem)
        {
            var vsproject = projectItem.ContainingProject.Object as VSProject;
            if (vsproject != null)
            {
                foreach (Reference reference in vsproject.References)
                {
                    AddProject(projectList, reference.SourceProject);
                }
            }
        }

        internal static void AddAllProjects(DTE dte, ICollection<string> projectList)
        {
            foreach (var project in dte.Solution.AllProjects())
            {
                AddProject(projectList, project);
            }
        }

        internal static IEnumerable<string> GetProjectItems(DTE dte, ICollection<string> projectList, string filter)
        {
            var directories = projectList.Select(p => new FileInfo(p).Directory);
            var files = directories.SelectMany(d => d.GetFiles(filter, SearchOption.AllDirectories));

            foreach (var file in files)
            {
                try
                {
                    if (dte.Solution.FindProjectItem(file.FullName) == null)
                        continue;
                }
                catch (Exception exception)
                {
                    Log.Debug($"Cannot find project item '{file.FullName}' ({exception.Message})");
                }

                yield return file.FullName;
            }
        }

        internal static bool ProjectListContainsItem(DTE dte, string filename, ICollection<string> projectList)
        {
            try
            {
                var projectItem = dte.Solution.FindProjectItem(filename);
                if (projectItem == null)
                    return false;

                return projectList.Contains(projectItem.ContainingProject.FullName);
            }
            catch (Exception exception)
            {
                Log.Debug($"Cannot find project item '{filename}' ({exception.Message})");
                return false;
            }
        }

        private static void AddProject(ICollection<string> projectList, Project project)
        {
            try
            {
                var filename = project?.FileName;
                if (filename != null && projectList.Contains(filename) == false)
                {
                    projectList.Add(filename);
                }
            }
            catch (Exception exception)
            {
                Log.Debug($"Cannot add project ({exception.Message})");
            }
        }
    }
}
