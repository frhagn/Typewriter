//using System;
//using System.ComponentModel.Design;
//using System.Globalization;
//using Microsoft.VisualStudio;
//using Microsoft.VisualStudio.Shell;
//using Microsoft.VisualStudio.Shell.Interop;
//using Typewriter.VisualStudio.Resources;

//namespace Typewriter.VisualStudio
//{
//    public class CommandManager
//    {
//        public CommandManager(GenerationManager generationManager)
//        {

//            funkar inte! -> var mcs = Package.GetGlobalService(typeof(IMenuCommandService)) as OleMenuCommandService;


//            // Add our command handlers for menu (commands must exist in the .vsct file)
//            //var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
//            if (null != mcs)
//            {
//                // Create the command for the menu item.
//                var menuCommandId = new CommandID(GuidList.VisualStudioExtensionCommandSet, (int)PkgCmdIdList.TypewriterCommand);
//                var menuItem = new MenuCommand(MenuItemCallback, menuCommandId);
//                mcs.AddCommand(menuItem);
//            }
//        }

//        /// <summary>
//        /// This function is the callback used to execute a command when the a menu item is clicked.
//        /// See the Initialize method to see how the menu item is associated to this function using
//        /// the OleMenuCommandService service and the MenuCommand class.
//        /// </summary>
//        private void MenuItemCallback(object sender, EventArgs e)
//        {
//            // Show a Message Box to prove we were here
//            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
//            //var uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
//            var clsid = Guid.Empty;
//            int result;
//            ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(0,ref clsid,"Typewriter",string.Format(CultureInfo.CurrentCulture, "!!!!! {0}.MenuItemCallback()", this.ToString()),string.Empty,0,OLEMSGBUTTON.OLEMSGBUTTON_OK,OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,OLEMSGICON.OLEMSGICON_INFO,0, /* false */out result));
//        }
//    }
//}