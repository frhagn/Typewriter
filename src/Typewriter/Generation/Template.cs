using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using EnvDTE;
using Typewriter.CodeModel.Configuration;
using Typewriter.Configuration;
using Typewriter.Generation.Controllers;
using Typewriter.VisualStudio;
using File = Typewriter.CodeModel.File;

namespace Typewriter.Generation
{
    public class Template
    {
        private readonly List<Type> _customExtensions = new List<Type>();
        private readonly string _templatePath;
        private readonly string _projectPath;
        private readonly string _projectFullName;
        private readonly ProjectItem _projectItem;
        private Lazy<string> _template;
        private Lazy<SettingsImpl> _configuration;
        private bool _templateCompileException;
        private bool _templateCompiled;
        
        public Settings Settings => _configuration.Value;

        public Template(ProjectItem projectItem)
        {
            var stopwatch = Stopwatch.StartNew();

            _projectItem = projectItem;
            _templatePath = projectItem.Path();
            _projectFullName = projectItem.ContainingProject.FullName;
            _projectPath = Path.GetDirectoryName(_projectFullName);


            _template = LazyTemplate();

            _configuration = LazyConfiguration();


            stopwatch.Stop();
            Log.Debug("Template ctor {0} ms", stopwatch.ElapsedMilliseconds);
        }

        private Lazy<SettingsImpl> LazyConfiguration()
        {

            return new Lazy<SettingsImpl>(() =>
           {
               var settings = new SettingsImpl(_projectItem);

               if (!_template.IsValueCreated)
               {
                    //force initialize template so _customExtensions will be loaded
                    var templateValue = _template.Value;
               }

               var templateClass = _customExtensions.FirstOrDefault();
               if (templateClass?.GetConstructor(new[] { typeof(Settings) }) != null)
               {
                   Activator.CreateInstance(templateClass, settings);
               }


               return settings;
           });
        }

        private Lazy<string> LazyTemplate()
        {
            _templateCompiled = false;
            _templateCompileException = false;

            return new Lazy<string>(() =>
            {
                var code = System.IO.File.ReadAllText(_templatePath);
                try
                {
                    var result = TemplateCodeParser.Parse(_projectItem, code, _customExtensions);
                    _templateCompiled = true;

                    return result;
                }
                catch (Exception)
                {
                    _templateCompileException = true;
                    throw;
                }
            });
        }

        public ICollection<string> GetFilesToRender()
        {
            var projects = _projectItem.DTE.Solution.AllProjects().Where(m => _configuration.Value.IncludedProjects.Any(p => m.FullName.Equals(p, StringComparison.OrdinalIgnoreCase)));
            return projects.SelectMany(m => m.AllProjectItems(Constants.CsExtension)).Select(m => m.Path()).ToList();
        }

        public bool ShouldRenderFile(string filename)
        {
            return ProjectHelpers.ProjectListContainsItem(_projectItem.DTE, filename, _configuration.Value.IncludedProjects);
        }

        public string Render(File file, out bool success)
        {
            try
            {
                return Parser.Parse(_projectItem, file.FullName, _template.Value, _customExtensions, file, out success);

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + " Template: " + _templatePath);
                success = false;
                return null;
            }
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
                    SaveFile(file, output, ref success);
                }
            }

            return success;
        }

        protected virtual void WriteFile(string outputPath, string outputContent)
        {
            System.IO.File.WriteAllText(outputPath, outputContent, new UTF8Encoding(true));
        }

        protected virtual void SaveFile(File file, string output, ref bool success)
        {
            ProjectItem item;
            var outputPath = GetOutputPath(file);

            if (string.Equals(file.FullName, outputPath, StringComparison.InvariantCultureIgnoreCase))
            {
                Log.Error("Output filename cannot match source filename.");
                success = false;
                return;
            }

            var hasChanged = HasChanged(outputPath, output);

            if (ExtensionPackage.Instance.Options.AddGeneratedFilesToProject == false)
            {
                if (hasChanged)
                {
                    WriteFile(outputPath, output);
                    Log.Debug($"Output file '{outputPath}' saved.");
                }
                return;
            }

            if (hasChanged)
            {
                CheckOutFileFromSourceControl(outputPath);
                WriteFile(outputPath, output);
                item = FindProjectItem(outputPath);

                if (item == null)
                {
                    try
                    {
                        item = _projectItem.ProjectItems.AddFromFile(outputPath);
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"Unable to add '{outputPath}' to project. {exception.Message}");
                    }
                }
            }
            else
            {
                item = FindProjectItem(outputPath);
            }

            SetMappedSourceFile(item, file.FullName);
        }

        public void DeleteFile(string path)
        {
            var item = GetExistingItem(path);
            item?.Delete();
        }

        public void RenameFile(File file, string oldPath, string newPath)
        {
            var item = GetExistingItem(oldPath);

            if (item != null)
            {
                if (Path.GetFileName(oldPath)?.Equals(Path.GetFileName(newPath)) ?? false)
                {
                    SetMappedSourceFile(item, newPath);
                    return;
                }

                var newOutputPath = GetOutputPath(file);

                item.Name = Path.GetFileName(newOutputPath);
                SetMappedSourceFile(item, newPath);
            }
        }

        private string GetMappedSourceFile(ProjectItem item)
        {
            if (item == null) return null;

            try
            {
                var value = item.Properties.Item("CustomToolNamespace").Value as string;
                return string.IsNullOrWhiteSpace(value) ? null : Path.GetFullPath(Path.Combine(_projectPath, value));
            }
            catch
            {
                return null;
            }
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
                try
                {
                    var property = item.Properties.Item("CustomToolNamespace");
                    if (property == null) throw new InvalidOperationException("Cannot find CustomToolNamespace property");

                    property.Value = relativeSourcePath;
                }
                catch
                {
                    Log.Warn("Failed to save source map for '{0}'. Project type not supported.", path);
                }
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
                    // Can't read properties from project item sometimes when deleting multiple files
                }
            }

            return null;
        }

        private string GetOutputPath(File file)
        {
            var path = file.FullName;
            var directory = Path.GetDirectoryName(_templatePath);
            var filename = GetOutputFilename(file, path);
            var outputPath = Path.Combine(directory, filename);

            for (var i = 1; i < 1000; i++)
            {
                var item = FindProjectItem(outputPath);
                if (item == null) return outputPath;

                var mappedSourceFile = GetMappedSourceFile(item);
                if (mappedSourceFile == null || path.Equals(mappedSourceFile, StringComparison.InvariantCultureIgnoreCase)) return outputPath;

                var name = filename.EndsWith(".d.ts", StringComparison.OrdinalIgnoreCase) ?
                    filename.Substring(0, filename.Length - 5) :
                    filename.Substring(0, filename.LastIndexOf(".", StringComparison.Ordinal));

                var extension = filename.EndsWith(".d.ts", StringComparison.OrdinalIgnoreCase) ?
                    ".d.ts" :
                    filename.Substring(filename.LastIndexOf(".", StringComparison.Ordinal));

                outputPath = Path.Combine(directory, $"{name} ({i}){extension}");
            }

            throw new Exception("GetOutputPath");
        }

        private string GetOutputFilename(File file, string sourcePath)
        {
            var sourceFilename = Path.GetFileNameWithoutExtension(sourcePath);
            var extension = GetOutputExtension();

            try
            {
                if (_configuration.Value.OutputFilenameFactory != null)
                {
                    var filename = _configuration.Value.OutputFilenameFactory(file);

                    filename = filename
                        .Replace("<", "-")
                        .Replace(">", "-")
                        .Replace(":", "-")
                        .Replace("\"", "-")
                        //.Replace("/", "-")
                        //.Replace("\\", "-")
                        .Replace("|", "-")
                        .Replace("?", "-")
                        .Replace("*", "-");

                    if (filename.Contains(".") == false)
                        filename += extension;

                    return filename;
                }
            }
            catch (Exception exception)
            {
                Log.Warn($"Can't get output filename for '{sourcePath}' ({exception.Message})");
            }

            return sourceFilename + extension;
        }

        private string GetOutputExtension()
        {
            var extension = _configuration.Value.OutputExtension;

            if (string.IsNullOrWhiteSpace(extension))
                return ".ts";

            return "." + extension.Trim('.');
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
                    // Can't read properties from project item sometimes when deleting multiple files
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

        public virtual void SaveProjectFile()
        {
            Log.Debug("Saving Project File: {0} ", _projectFullName);
            var stopwatch = Stopwatch.StartNew();

            _projectItem.ContainingProject.Save();

            stopwatch.Stop();
            Log.Debug("SaveProjectFile completed in {0} ms", stopwatch.ElapsedMilliseconds);
        }

        public string ProjectFullName { get { return _projectFullName; } }

        public bool IsCompiled
        {
            get { return _templateCompiled; }
        }

        public bool HasCompileException
        {
            get { return _templateCompileException; }
        }

        public string TemplatePath
        {
            get { return _templatePath; }
        }

        public void Reload()
        {
            _template = LazyTemplate();
            _configuration = LazyConfiguration();
        }
    }
}
