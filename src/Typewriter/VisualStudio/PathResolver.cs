using System;
using System.IO;
using EnvDTE;

namespace Typewriter.VisualStudio
{
    public static class PathResolver
    {
        public static string ResolveRelative(string path, ProjectItem projectItem)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (Path.IsPathRooted(path) || projectItem == null) return path;

            if (path.StartsWith("~\\"))
            {
                var folder = Path.GetDirectoryName(projectItem.ContainingProject.FullName);
                return Path.Combine(folder, path.Substring(2));
            }
            else if (path.StartsWith("~~\\"))
            {
                var folder = Path.GetDirectoryName(projectItem.DTE.Solution.FullName);
                return Path.Combine(folder, path.Substring(3));
            }
            else
            {
                var folder = Path.GetDirectoryName(projectItem.Document.Path);
                return Path.Combine(folder, path);
            }
        }
    }
}