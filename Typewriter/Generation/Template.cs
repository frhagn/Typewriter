using System;
using System.IO;
using EnvDTE;
using Typewriter.CodeModel;

namespace Typewriter.Generation
{
    public interface ITemplate
    {
        void Render(IFileInfo file);
        void DeleteFile(string path);
        void RenameFile(string oldPath, string newPath);
        void VerifyProjectItem();
    }

    public class Template : ITemplate
    {
        private readonly string template;
        private readonly string templatePath;
        private readonly string solutionPath;
        private readonly ProjectItem projectItem;

        public Template(ProjectItem projectItem)
        {
            this.projectItem = projectItem;
            this.templatePath = projectItem.Properties.Item("FullPath").Value.ToString();
            this.solutionPath = Path.GetDirectoryName(projectItem.DTE.Solution.FullName) + @"\";

            //string codePath = null;
            //foreach (ProjectItem item in projectItem.ProjectItems)
            //{
            //    if (item.Name.EndsWith(".tst.cs", StringComparison.InvariantCultureIgnoreCase))
            //    {
            //        codePath = item.FileNames[1];
            //        break;
            //    }
            //}

            this.template = File.ReadAllText(templatePath);
            //this.extensions = (codePath != null) ? Compiler.Compile(File.ReadAllText(codePath)) : typeof(Extensions);
        }

        public void Render(IFileInfo file)
        {
            var parser = new Parser(); //new Parser(extensions);
            var output = parser.Parse(template, file);

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
            item.ContainingProject.Save();
        }

        public void DeleteFile(string path)
        {
            var item = GetExistingItem(path);

            if (item != null)
            {
                var lastItem = FindLastProjectItem(path);
                if (lastItem != null && lastItem != item)
                {
                    var outputPath = item.Properties.Item("FullPath").Value.ToString();

                    File.Delete(outputPath);
                    File.Move(lastItem.Properties.Item("FullPath").Value.ToString(), outputPath);
                    SetMappedSourceFile(item, GetMappedSourceFile(lastItem));
                    lastItem.Remove();
                }
                else
                {
                    item.Delete();
                }

                item.ContainingProject.Save();
            }
        }

        public void RenameFile(string oldPath, string newPath)
        {
            if (Path.GetFileName(oldPath).Equals(Path.GetFileName(newPath))) return;

            var item = GetExistingItem(oldPath);

            if (item != null)
            {
                var newOutputPath = GetOutputPath(newPath);
                var oldOutputPath = item.Properties.Item("FullPath").Value.ToString();
                var lastItem = FindLastProjectItem(oldPath);

                File.Move(oldOutputPath, newOutputPath);
                var newItem = projectItem.ProjectItems.AddFromFile(newOutputPath);
                SetMappedSourceFile(newItem, newPath);

                if (lastItem != null && lastItem != item)
                {
                    File.Move(lastItem.Properties.Item("FullPath").Value.ToString(), oldOutputPath);
                    SetMappedSourceFile(item, GetMappedSourceFile(lastItem));
                    lastItem.Remove();
                }
                else
                {
                    item.Remove();
                }

                newItem.ContainingProject.Save();
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
                    var itemPath = item.Properties.Item("FullPath").Value.ToString();
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
