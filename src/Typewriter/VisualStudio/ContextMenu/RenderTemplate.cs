//------------------------------------------------------------------------------
// <copyright file="RenderTemplate.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.IO;
using System.Runtime.InteropServices;
using Typewriter.Generation.Controllers;
using Typewriter.Generation;
using System.Linq;

namespace Typewriter.VisualStudio.ContextMenu
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class RenderTemplate
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int RenderOneCommandId = 0x0100;
        public const int RenderAllCommandId = 0x0101;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("7d06724e-3bb6-47e7-99d2-bb4ba983191d");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        private string itemFullPath;

        public event SingleFileChangedEventHandler RenderTemplateClicked;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTemplate"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private RenderTemplate(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, RenderOneCommandId);

                var menuItem = new OleMenuCommand(MenuItemCallbackRenderOne, menuCommandID);
                menuItem.BeforeQueryStatus += menuCommand_BeforeQueryStatus;

                commandService.AddCommand(menuItem);
            }

            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, RenderAllCommandId);

                var menuItem = new OleMenuCommand(MenuItemCallbackRenderAll, menuCommandID);
                menuItem.BeforeQueryStatus += menuCommand_BeforeQueryStatus;

                commandService.AddCommand(menuItem);
            }
        }

        private void menuCommand_BeforeQueryStatus(object sender, EventArgs e)
        {
            // get the menu that fired the event
            var menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {
                // start by assuming that the menu will not be shown
                menuCommand.Visible = false;
                menuCommand.Enabled = false;

                IVsHierarchy hierarchy = null;
                uint itemid = VSConstants.VSITEMID_NIL;

                if (!IsSingleProjectItemSelection(out hierarchy, out itemid)) return;
                // Get the file path
                itemFullPath = null;
                ((IVsProject)hierarchy).GetMkDocument(itemid, out itemFullPath);
                var transformFileInfo = new FileInfo(itemFullPath);

                bool isTemplate = transformFileInfo.Name.EndsWith(".tst");

                // if not leave the menu hidden
                if (!isTemplate) return;

                menuCommand.Visible = true;
                menuCommand.Enabled = true;
            }
        }

        private bool IsSingleProjectItemSelection(out IVsHierarchy hierarchy, out uint itemid)
        {
            hierarchy = null;
            itemid = VSConstants.VSITEMID_NIL;
            int hr = VSConstants.S_OK;

            var monitorSelection = Package.GetGlobalService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
            var solution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
            if (monitorSelection == null || solution == null)
            {
                return false;
            }

            IVsMultiItemSelect multiItemSelect = null;
            IntPtr hierarchyPtr = IntPtr.Zero;
            IntPtr selectionContainerPtr = IntPtr.Zero;

            try
            {
                hr = monitorSelection.GetCurrentSelection(out hierarchyPtr, out itemid, out multiItemSelect, out selectionContainerPtr);

                if (ErrorHandler.Failed(hr) || hierarchyPtr == IntPtr.Zero || itemid == VSConstants.VSITEMID_NIL)
                {
                    // there is no selection
                    return false;
                }

                // multiple items are selected
                if (multiItemSelect != null) return false;

                // there is a hierarchy root node selected, thus it is not a single item inside a project

                if (itemid == VSConstants.VSITEMID_ROOT) return false;

                hierarchy = Marshal.GetObjectForIUnknown(hierarchyPtr) as IVsHierarchy;
                if (hierarchy == null) return false;

                Guid guidProjectID = Guid.Empty;

                if (ErrorHandler.Failed(solution.GetGuidOfProject(hierarchy, out guidProjectID)))
                {
                    return false; // hierarchy is not a project inside the Solution if it does not have a ProjectID Guid
                }

                // if we got this far then there is a single project item selected
                return true;
            }
            finally
            {
                if (selectionContainerPtr != IntPtr.Zero)
                {
                    Marshal.Release(selectionContainerPtr);
                }

                if (hierarchyPtr != IntPtr.Zero)
                {
                    Marshal.Release(hierarchyPtr);
                }
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static RenderTemplate Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new RenderTemplate(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallbackRenderOne(object sender, EventArgs e)
        {
            //string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
            //string title = "RenderTemplate";

            var renderTemplateClicked = RenderTemplateClicked;
            renderTemplateClicked?.Invoke(this, new SingleFileChangedEventArgs(FileChangeType.Changed, itemFullPath));
        }

        private void MenuItemCallbackRenderAll(object sender, EventArgs e)
        {
            var renderTemplateClicked = RenderTemplateClicked;

            // trigger a change and have it queue up
            var helper = new SolutionFilesHelper();
            var templates = helper.SolutionFiles().Select(x => x.Name).Where(x => x.EndsWith(Constants.TemplateExtension));
            foreach (var itm in templates)
            {
                Log.Debug($"Invoke renderTemplateClicked for '{itm}'.");
                renderTemplateClicked?.Invoke(this, new SingleFileChangedEventArgs(FileChangeType.Changed, itm));
            }
        }
    }
}
