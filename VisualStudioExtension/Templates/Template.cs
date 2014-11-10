using System;
using System.IO;
using EnvDTE;
using Typewriter.CodeModel;

namespace Typewriter.Templates
{
    public interface ITemplate
    {
        void Render(IFileInfo fileInfo);
        void DeleteFile(string path);
        void RenameFile(string oldPath, string newPath);
    }

    public class Template : ITemplate
    {
        private readonly string template;
        private readonly Type extensions;
        private readonly string templatePath;

        public ProjectItem ProjectItem { get; set; }

        public Template(ProjectItem projectItem)
        {
            ProjectItem = projectItem;
            templatePath = projectItem.FileNames[1];

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

        public void Render(IFileInfo fileInfo)
        {
            var parser = new Parser(extensions);
            var output = parser.Parse(template, fileInfo);
            var outputPath = GetOutputPath(fileInfo.FullName);

            if (output == null)
            {
                DeleteFile(outputPath);
            }
            else if (HasChanged(outputPath, output))
            {
                SaveFile(outputPath, output);
            }
        }

        private void SaveFile(string path, string output)
        {
            File.WriteAllText(path, output);
            ProjectItem.ProjectItems.AddFromFile(path);
        }

        public void DeleteFile(string path)
        {
            var outputPath = GetOutputPath(path);
            var item = FindProjectItem(ProjectItem, outputPath);
            if (item != null)
            {
                item.Delete();
            }
        }

        public void RenameFile(string oldPath, string newPath)
        {
            var oldOutputPath = GetOutputPath(oldPath);
            var newOutputPath = GetOutputPath(newPath);
            var item = FindProjectItem(ProjectItem, oldOutputPath);

            if (item != null)
            {
                item.Remove();
                File.Move(oldOutputPath, newOutputPath);
                ProjectItem.ProjectItems.AddFromFile(newOutputPath);
            }
        }

        private string GetOutputPath(string path)
        {
            var directory = Path.GetDirectoryName(templatePath);
            var fileName = Path.GetFileNameWithoutExtension(path) + ".ts";

            return Path.Combine(directory, fileName);
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

        private static ProjectItem FindProjectItem(ProjectItem projectItem, string path)
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
    }
}
