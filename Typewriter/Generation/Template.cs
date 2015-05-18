using System;
using System.Diagnostics;
using System.IO;
using EnvDTE;
using Typewriter.Generation.Controllers;
using Typewriter.VisualStudio;
using File = Typewriter.CodeModel.File;

namespace Typewriter.Generation
{
    public class Template
    {
        private readonly object locker = new object();

        private readonly Type extensions;
        private readonly string template;
        private readonly string templatePath;
        private readonly string projectPath;
        private readonly string solutionPath;
        private readonly ProjectItem projectItem;

        public Template(ProjectItem projectItem)
        {
            var stopwatch = Stopwatch.StartNew();

            this.projectItem = projectItem;
            this.templatePath = projectItem.Path();
            this.projectPath = Path.GetDirectoryName(projectItem.ContainingProject.FullName);
            this.solutionPath = Path.GetDirectoryName(projectItem.DTE.Solution.FullName) + @"\";
            
            var code = System.IO.File.ReadAllText(templatePath);
            this.template = TemplateParser.Parse(code, ref this.extensions);

            stopwatch.Stop();
            Log.Debug("Template ctor {0} ms", stopwatch.ElapsedMilliseconds);
        }

        public string Render(File file, out bool success)
        {
            return Parser.Parse(template, extensions, file, out success);
        }

        public bool RenderFile(File file, bool saveProjectFile)
        {
            bool success;
            var output = Render(file, out success);

            if (success)
            {
                if (output == null)
                {
                    DeleteFile(file.FullName, saveProjectFile);
                }
                else
                {
                    SaveFile(file.FullName, output, saveProjectFile);
                }
            }

            return success;
        }

        private void SaveFile(string path, string output, bool saveProjectFile)
        {
            ProjectItem item;
            var outputPath = GetOutputPath(path);

            if (HasChanged(outputPath, output))
            {
                System.IO.File.WriteAllText(outputPath, output);
                item = FindProjectItem(outputPath) ?? projectItem.ProjectItems.AddFromFile(outputPath);
            }
            else
            {
                item = FindProjectItem(outputPath);
            }

            SetMappedSourceFile(item, path);
            if (saveProjectFile)
                projectItem.ContainingProject.Save();
        }

        public void DeleteFile(string path, bool saveProjectFile)
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

                        System.IO.File.Delete(outputPath);
                        System.IO.File.Move(lastItem.Path(), outputPath);
                        SetMappedSourceFile(item, GetMappedSourceFile(lastItem));
                        lastItem.Remove();
                    }
                    else
                    {
                        item.Delete();
                    }

                    if (saveProjectFile)
                        projectItem.ContainingProject.Save();
                }
            }
        }

        public void RenameFile(string oldPath, string newPath, bool saveProjectFile)
        {
            lock (locker)
            {
                var item = GetExistingItem(oldPath);

                if (item != null)
                {
                    if (Path.GetFileName(oldPath).Equals(Path.GetFileName(newPath)))
                    {
                        SetMappedSourceFile(item, newPath);
                        if (saveProjectFile)
                            projectItem.ContainingProject.Save();
                        return;
                    }

                    var newOutputPath = GetOutputPath(newPath);
                    var oldOutputPath = item.Path();
                    var lastItem = FindLastProjectItem(oldPath);

                    if (lastItem != null && lastItem != item)
                    {
                        System.IO.File.Move(oldOutputPath, newOutputPath);
                        var newItem = projectItem.ProjectItems.AddFromFile(newOutputPath);
                        SetMappedSourceFile(newItem, newPath);

                        System.IO.File.Move(lastItem.Path(), oldOutputPath);
                        SetMappedSourceFile(item, GetMappedSourceFile(lastItem));
                        lastItem.Remove();
                    }
                    else
                    {
                        System.IO.File.Move(oldOutputPath, newOutputPath);
                        var newItem = projectItem.ProjectItems.AddFromFile(newOutputPath);
                        SetMappedSourceFile(newItem, newPath);
                        item.Remove();
                    }

                    if (saveProjectFile)
                        projectItem.ContainingProject.Save();
                }
            }
        }

        private string GetMappedSourceFile(ProjectItem item)
        {
            if (item == null) return null;

            var value = item.Properties.Item("CustomToolNamespace").Value as string;
            var path = string.IsNullOrWhiteSpace(value) ? null : Path.GetFullPath(Path.Combine(projectPath, value));

            // Handle files created using older Typewriter versions
            if (path != null && System.IO.File.Exists(path) == false)
            {
                return Path.Combine(solutionPath, value);
            }

            return path;
        }

        private void SetMappedSourceFile(ProjectItem item, string path)
        {
            var pathUri = new Uri(path);
            var folderUri = new Uri(projectPath.Trim(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);
            var relativeSourcePath = Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));

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

                outputPath = Path.Combine(directory, $"{fileName} ({i}).ts");
            }

            throw new Exception("GetOutputPath");
        }

        private static bool HasChanged(string path, string output)
        {
            if (System.IO.File.Exists(path))
            {
                var current = System.IO.File.ReadAllText(path);
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
