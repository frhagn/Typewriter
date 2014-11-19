using System;
using System.IO;
using EnvDTE;
using Typewriter.CodeModel;

namespace Typewriter.Templates
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
        private readonly Type extensions;
        private readonly string templatePath;
        private readonly string solutionPath;
        private readonly ProjectItem projectItem;

        public Template(ProjectItem projectItem)
        {
            this.projectItem = projectItem;
            this.templatePath = projectItem.FileNames[1];
            this.solutionPath = Path.GetDirectoryName(projectItem.DTE.Solution.FullName) + @"\";

            string codePath = null;
            //foreach (ProjectItem item in projectItem.ProjectItems)
            //{
            //    if (item.Name.EndsWith(".tst.cs", StringComparison.InvariantCultureIgnoreCase))
            //    {
            //        codePath = item.FileNames[1];
            //        break;
            //    }
            //}

            this.template = File.ReadAllText(templatePath);
            this.extensions = (codePath != null) ? Compiler.Compile(File.ReadAllText(codePath)) : typeof(Extensions);
        }

        public void Render(IFileInfo file)
        {
            var parser = new Parser(extensions);
            var output = parser.Parse(template, file);
            var outputPath = GetOutputPath(file.FullName);

            if (output == null)
            {
                DeleteFile(outputPath);
            }
            else 
            {
                SaveFile(outputPath, output, file.FullName);
            }
        }

        private void SaveFile(string path, string output, string fileName)
        {
            ProjectItem item;

            if (HasChanged(path, output))
            {
                File.WriteAllText(path, output);
                item = FindProjectItem(path) ?? projectItem.ProjectItems.AddFromFile(path);
            }
            else
            {
                item = FindProjectItem(path);
            }

            SetMappedSourceFile(item, fileName);
        }

        public void DeleteFile(string path)
        {
            var outputPath = GetOutputPath(path);
            var item = FindProjectItem(outputPath);

            if (item != null)
            {
                var lastItem = FindLastProjectItem(path);

                if (lastItem != null && lastItem != item)
                {
                    File.Delete(item.FileNames[1]);
                    File.Move(lastItem.FileNames[1], item.FileNames[1]);
                    SetMappedSourceFile(item, GetMappedSourceFile(lastItem));
                    lastItem.Remove();
                }
                else
                {
                    item.Delete();
                }
            }
        }

        public void RenameFile(string oldPath, string newPath)
        {
            var oldOutputPath = GetOutputPath(oldPath);
            var newOutputPath = GetOutputPath(newPath);
            var item = FindProjectItem(oldOutputPath);

            if (item != null)
            {
                var lastItem = FindLastProjectItem(oldPath);
                File.Move(oldOutputPath, newOutputPath);

                if (lastItem != null && lastItem != item)
                {
                    File.Move(lastItem.FileNames[1], item.FileNames[1]);
                    SetMappedSourceFile(item, GetMappedSourceFile(lastItem));
                    lastItem.Remove();
                }
                else
                {
                    item.Remove();
                }

                var newItem = projectItem.ProjectItems.AddFromFile(newOutputPath);
                SetMappedSourceFile(newItem, newPath);
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
                var itemPath = item.Properties.Item("FullPath").Value.ToString();
                if (itemPath.Equals(path, StringComparison.InvariantCultureIgnoreCase))
                {
                    return item;
                }
            }

            return null;
        }

        private ProjectItem FindLastProjectItem(string path)
        {
            ProjectItem lastItem = null;
            var directory = Path.GetDirectoryName(templatePath);
            var filename = Path.GetFileNameWithoutExtension(path);


            for (var i = 1; i < projectItem.ProjectItems.Count; i++)
            {
                var indexedName = (i == 0) ? string.Format("{0} ({1})", filename, i) : filename;
                var outputPath = Path.Combine(directory, indexedName) + ".ts";
                var item = FindProjectItem(outputPath);

                if (item != null) lastItem = item;
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
