using System;
using System.IO;
using EnvDTE;
using Typewriter.CodeModel;
using Typewriter.Generation.Controllers;

namespace Typewriter.Generation
{
    public class Template
    {
        private readonly object locker = new object();

        private readonly Type extensions;
        private readonly string template;
        private readonly string templatePath;
        private readonly string solutionPath;
        private readonly ProjectItem projectItem;

        public Template(ProjectItem projectItem)
        {
            this.projectItem = projectItem;
            this.templatePath = projectItem.Path();
            this.solutionPath = Path.GetDirectoryName(projectItem.DTE.Solution.FullName) + @"\";

            var code = File.ReadAllText(templatePath);
            this.template = TemplateParser.Parse(code, ref this.extensions);
        }

        public void Render(IFileInfo file)
        {
            var output = Parser.Parse(template, extensions, file);

            if (output == null)
            {
                DeleteFile(file.FullName);
            }
            else
            {
                SaveFile(file.FullName, output);
            }
        }

        private void SaveFile(string path, string output)
        {
            ProjectItem item;
            var outputPath = GetOutputPath(path);

            if (HasChanged(outputPath, output))
            {
                File.WriteAllText(outputPath, output);
                item = FindProjectItem(outputPath) ?? projectItem.ProjectItems.AddFromFile(outputPath);
            }
            else
            {
                item = FindProjectItem(outputPath);
            }

            SetMappedSourceFile(item, path);
            projectItem.ContainingProject.Save();
        }

        public void DeleteFile(string path)
        {
            lock (locker)
            {
                var item = GetExistingItem(path);

                if (item != null)
                {
                    var lastItem = FindLastProjectItem(path);
                    if (lastItem != null && lastItem != item)
                    {
                        var outputPath = item.Path();

                        File.Delete(outputPath);
                        File.Move(lastItem.Path(), outputPath);
                        SetMappedSourceFile(item, GetMappedSourceFile(lastItem));
                        lastItem.Remove();
                    }
                    else
                    {
                        item.Delete();
                    }

                    projectItem.ContainingProject.Save();
                }
            }
        }

        public void RenameFile(string oldPath, string newPath)
        {
            lock(locker)
            {
            var item = GetExistingItem(oldPath);

                if (item != null)
                {
                    if (Path.GetFileName(oldPath).Equals(Path.GetFileName(newPath)))
                    {
                        SetMappedSourceFile(item, newPath);
                        projectItem.ContainingProject.Save();
                        return;
                    }

                    var newOutputPath = GetOutputPath(newPath);
                    var oldOutputPath = item.Path();
                    var lastItem = FindLastProjectItem(oldPath);

                    if (lastItem != null && lastItem != item)
                    {
                        File.Move(oldOutputPath, newOutputPath);
                        var newItem = projectItem.ProjectItems.AddFromFile(newOutputPath);
                        SetMappedSourceFile(newItem, newPath);

                        File.Move(lastItem.Path(), oldOutputPath);
                        SetMappedSourceFile(item, GetMappedSourceFile(lastItem));
                        lastItem.Remove();

                        projectItem.ContainingProject.Save();
                    }
                    else
                    {
                        item.SaveAs(newOutputPath);
                        SetMappedSourceFile(item, newPath);
                        projectItem.ContainingProject.Save();
                    }
                }
            }
        }

        private string GetRelativeSourcePath(string path)
        {
            if (path.StartsWith(solutionPath, StringComparison.InvariantCultureIgnoreCase))
            {
                path = path.Remove(0, solutionPath.Length);
            }

            return path;
        }

        private string GetMappedSourceFile(ProjectItem item)
        {
            if (item == null) return null;

            var value = item.Properties.Item("CustomToolNamespace").Value as string;
            return string.IsNullOrWhiteSpace(value) ? null : Path.Combine(solutionPath, value);
        }

        private void SetMappedSourceFile(ProjectItem item, string path)
        {
            var relativeSourcePath = GetRelativeSourcePath(path);
            if (relativeSourcePath.Equals(GetMappedSourceFile(item), StringComparison.InvariantCultureIgnoreCase) == false)
            {
                item.Properties.Item("CustomToolNamespace").Value = relativeSourcePath;
            }
        }

        private ProjectItem GetExistingItem(string path)
        {
            foreach (ProjectItem item in projectItem.ProjectItems)
            {
                try
                {
                    if (path.Equals(GetMappedSourceFile(item), StringComparison.InvariantCultureIgnoreCase))
                    {
                        return item;
                    }
                }
                catch
                {
                    // Can't read properties from project item sometimes when deleting miltiple files
                }
            }

            return null;
        }

        private string GetOutputPath(string path)
        {
            var directory = Path.GetDirectoryName(templatePath);
            var fileName = Path.GetFileNameWithoutExtension(path);
            var outputPath = Path.Combine(directory, fileName) + ".ts";

            for (var i = 1; i < 1000; i++)
            {
                var item = FindProjectItem(outputPath);
                if (item == null) return outputPath;

                var mappedSourceFile = GetMappedSourceFile(item);
                if (mappedSourceFile == null || path.Equals(mappedSourceFile, StringComparison.InvariantCultureIgnoreCase)) return outputPath;

                outputPath = Path.Combine(directory, string.Format("{0} ({1}).ts", fileName, i));
            }

            throw new Exception("GetOutputPath");
        }

        private static bool HasChanged(string path, string output)
        {
            if (File.Exists(path))
            {
                var current = File.ReadAllText(path);
                if (current == output)
                {
                    return false;
                }
            }

            return true;
        }

        private ProjectItem FindProjectItem(string path)
        {
            foreach (ProjectItem item in projectItem.ProjectItems)
            {
                try
                {
                    var itemPath = item.Path();
                    if (itemPath.Equals(path, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return item;
                    }
                }
                catch
                {
                    // Can't read properties from project item sometimes when deleting miltiple files
                }
            }

            return null;
        }

        private ProjectItem FindLastProjectItem(string path)
        {
            ProjectItem lastItem = null;
            var directory = Path.GetDirectoryName(templatePath);
            var filename = Path.GetFileNameWithoutExtension(path);

            for (var i = 0; i < projectItem.ProjectItems.Count; i++)
            {
                try
                {
                    var indexedName = (i > 0) ? string.Format("{0} ({1})", filename, i) : filename;
                    var outputPath = Path.Combine(directory, indexedName) + ".ts";
                    var item = FindProjectItem(outputPath);

                    if (item != null) lastItem = item;
                }
                catch
                {
                    // Can't read properties from project item sometimes when deleting miltiple files
                }
            }

            return lastItem;
        }

        public void VerifyProjectItem()
        {
            // ReSharper disable once UnusedVariable
            var dummy = projectItem.FileNames[1];
        }
    }
}
