using System;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace Typewriter.VisualStudio
{
    public static class PathResolver
    {
        public static string ResolveRelative(string path, ProjectItem projectItem)
        {
            return ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                if (path == null)
                    throw new ArgumentNullException(nameof(path));

                if (Path.IsPathRooted(path) || projectItem == null) return path;

                if (path.StartsWith("~\\", StringComparison.OrdinalIgnoreCase))
                {
                    var folder = Path.GetDirectoryName(projectItem.ContainingProject.FullName);
                    return Path.Combine(folder, path.Substring(2));
                }
                else if (path.StartsWith("~~\\", StringComparison.OrdinalIgnoreCase))
                {
                    var folder = Path.GetDirectoryName(projectItem.DTE.Solution.FullName);
                    return Path.Combine(folder, path.Substring(3));
                }
                else
                {
                    var folder = Path.GetDirectoryName(projectItem.Document.Path);
                    return Path.Combine(folder, path);
                }
            });
        }
    }
}