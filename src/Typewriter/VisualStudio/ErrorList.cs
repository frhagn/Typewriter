using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Typewriter.VisualStudio
{
    internal static class ErrorList
    {
        private static IVsSolution _ivsSolution;
        private static IVsErrorList _ivsErrorList;
        private static ErrorListProvider _errorListProvider;
        private static bool _initialized;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            try
            {
                _ivsSolution = (IVsSolution)Package.GetGlobalService(typeof (IVsSolution));
                _ivsErrorList = (IVsErrorList)Package.GetGlobalService(typeof (SVsErrorList));
                _errorListProvider = new ErrorListProvider(serviceProvider);
                _initialized = true;
            }
            catch (Exception exception)
            {
                Log.Error($"Failed to initialize Error List. {exception.Message}");
            }
        }

        public static void Clear()
        {
            if (_initialized == false) return;

            try
            {
                _errorListProvider.Tasks.Clear();
            }
            catch (Exception exception)
            {
                Log.Error($"Failed to clear Error List. {exception.Message}");
            }
        }

        public static void Show()
        {
            if (_initialized == false) return;

            try
            {
                _errorListProvider.Show();
                _errorListProvider.ForceShowErrors();
                _ivsErrorList.BringToFront();
            }
            catch (Exception exception)
            {
                Log.Error($"Failed to show Error List. {exception.Message}");
            }
        }

        public static void AddError(ProjectItem projectItem, string message, int line = -1)
        {
            AddTask(projectItem, message, TaskErrorCategory.Error, line);
        }

        public static void AddWarning(ProjectItem projectItem, string message, int line = -1)
        {
            AddTask(projectItem, message, TaskErrorCategory.Warning, line);
        }

        public static void AddMessage(ProjectItem projectItem, string message, int line = -1)
        {
            AddTask(projectItem, message, TaskErrorCategory.Message, line);
        }

        private static void AddTask(ProjectItem projectItem, string message, TaskErrorCategory category, int line)
        {
            if (_initialized == false) return;

            try
            {
                var project = projectItem.ContainingProject.FileName;
                var filename = projectItem.FileNames[0];

                IVsHierarchy hierarchyItem;
                _ivsSolution.GetProjectOfUniqueName(project, out hierarchyItem);

                var errorTask = new ErrorTask
                {
                    Category = TaskCategory.BuildCompile,
                    ErrorCategory = category,
                    Text = message,
                    Document = filename,
                    Line = line,
                    HierarchyItem = hierarchyItem
                };

                errorTask.Navigate += (sender, e) =>
                {
                    //there are two Bugs in the errorListProvider.Navigate method:
                    //    Line number needs adjusting
                    //    Column is not shown
                    errorTask.Line++;
                    var guid = new Guid(EnvDTE.Constants.vsViewKindCode);
                    _errorListProvider.Navigate(errorTask, guid);
                    errorTask.Line--;
                };

                _errorListProvider.Tasks.Add(errorTask);
            }
            catch (Exception exception)
            {
                Log.Error($"Failed to add task to Error List. {exception.Message}");
            }
        }
    }
}