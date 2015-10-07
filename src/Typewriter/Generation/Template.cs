using System;
using System.Collections.Generic;
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
        private readonly List<Type> _customExtensions = new List<Type>();
        private readonly string _template;
        private readonly string _templatePath;
        private readonly string _projectPath;
        private readonly ProjectItem _projectItem;

        public Template(ProjectItem projectItem)
        {
            var stopwatch = Stopwatch.StartNew();

            _projectItem = projectItem;
            _templatePath = projectItem.Path();
            _projectPath = Path.GetDirectoryName(projectItem.ContainingProject.FullName);

            var code = System.IO.File.ReadAllText(_templatePath);
            _template = TemplateCodeParser.Parse(code, _customExtensions);

            stopwatch.Stop();
            Log.Debug("Template ctor {0} ms", stopwatch.ElapsedMilliseconds);
        }

        public string Render(File file, out bool success)
        {
            return Parser.Parse(_template, _customExtensions, file, out success);
        }

        public bool RenderFile(File file)
        {
            bool success;
            var output = Render(file, out success);

            if (success)
            {
                if (output == null)
                {
                    DeleteFile(file.FullName);
                }
                else
                {
                    SaveFile(file.FullName, output);
                }
            }

            return success;
        }

        private void SaveFile(string path, string output)
        {
            
            ProjectItem item;
            var outputPath = GetOutputPath(path);

            if (HasChanged(outputPath, output))
            {
                CheckOutFileFromSourceControl(outputPath);

                System.IO.File.WriteAllText(outputPath, output);
                item = FindProjectItem(outputPath) ?? _projectItem.ProjectItems.AddFromFile(outputPath);
            }
            else
            {
                item = FindProjectItem(outputPath);
            }

            SetMappedSourceFile(item, path);
            
            
        }

        public void DeleteFile(string path)
        {
            
            var item = GetExistingItem(path);

            if (item != null)
            {
                item.Delete();
                

            }
            
        }

        public void RenameFile(string oldPath, string newPath)
        {
            var item = GetExistingItem(oldPath);

            if (item != null)
            {
                if (Path.GetFileName(oldPath)?.Equals(Path.GetFileName(newPath)) ?? false)
                {
                    SetMappedSourceFile(item, newPath);
                    
                    return;
                }

                var newOutputPath = GetOutputPath(newPath);

                item.Name = Path.GetFileName(newOutputPath);
                SetMappedSourceFile(item, newPath);

            }
            
        }

        private string GetMappedSourceFile(ProjectItem item)
        {
            if (item == null) return null;

            var value = item.Properties.Item("CustomToolNamespace").Value as string;
            var path = string.IsNullOrWhiteSpace(value) ? null : Path.GetFullPath(Path.Combine(_projectPath, value));

            return path;
        }

        private void SetMappedSourceFile(ProjectItem item, string path)
        {
            if (_projectItem == null) throw new ArgumentException("item");
            if (path == null) throw new ArgumentException("path");

            var pathUri = new Uri(path);
            var folderUri = new Uri(_projectPath.Trim(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);
            var relativeSourcePath = Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));

            if (relativeSourcePath.Equals(GetMappedSourceFile(item), StringComparison.InvariantCultureIgnoreCase) == false)
            {
                var property = item.Properties.Item("CustomToolNamespace");
                if (property == null) throw new InvalidOperationException("Cannot find CustomToolNamespace property");

                property.Value = relativeSourcePath;
            }
        }

        private ProjectItem GetExistingItem(string path)
        {
            foreach (ProjectItem item in _projectItem.ProjectItems)
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
            var directory = Path.GetDirectoryName(_templatePath);
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
            foreach (ProjectItem item in _projectItem.ProjectItems)
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

        private void CheckOutFileFromSourceControl(string path)
        {
            try
            {
                var dte = _projectItem.DTE;
                var fileExists = System.IO.File.Exists(path) && dte.Solution.FindProjectItem(path) != null;
                var isUnderScc = dte.SourceControl.IsItemUnderSCC(path);
                var isCheckedOut = dte.SourceControl.IsItemCheckedOut(path);

                if (fileExists && isUnderScc && isCheckedOut == false)
                {
                    dte.SourceControl.CheckOutItem(path);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
            }
        }

        public void VerifyProjectItem()
        {
            // ReSharper disable once UnusedVariable
            var dummy = _projectItem.FileNames[1];
        }

        public void SaveProjectFile()
        {
            Log.Debug("Saving Project File: {0} ", _projectItem.ContainingProject.FullName);
            _projectItem.ContainingProject.Save();
        }
    }
}
