using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using EnvDTE;
using File = Typewriter.CodeModel.File;

namespace Tests
{
    public abstract class TestBase
    {
        protected static DTE dte;

        [DllImport("ole32.dll")]
        public static extern int GetRunningObjectTable(uint dwReserved, out IRunningObjectTable pprot);

        [DllImport("ole32.dll")]
        public static extern int CreateBindCtx(uint dwReserved, out IBindCtx ppbc);

        static TestBase()
        {
            dte = GetDte("Typewriter.sln");
        }
        
        protected static string GetSolutionDirectory()
        {
            return new FileInfo(dte.Solution.FileName).Directory.FullName;
        }

        protected static ProjectItem GetProjectItem(string path)
        {
            var solution = GetSolutionDirectory();
            return dte.Solution.FindProjectItem(Path.Combine(solution, path));
        }

        protected static string GetFileContents(string path)
        {
            var solution = GetSolutionDirectory();
            return System.IO.File.ReadAllText(Path.Combine(solution, path));
        }

        protected static File GetFile(string path)
        {
            return new Typewriter.CodeModel.CodeDom.FileInfo(GetProjectItem(path));
        }

        private static DTE GetDte(string solution)
        {
            const string visualStudioProgId = "!VisualStudio.DTE.";

            IRunningObjectTable runningObjectTable = null;
            IEnumMoniker enumMoniker = null;
            IBindCtx bindCtx = null;

            try
            {
                Marshal.ThrowExceptionForHR(GetRunningObjectTable(0, out runningObjectTable));
                runningObjectTable.EnumRunning(out enumMoniker);

                IMoniker[] monikers = new IMoniker[1];
                enumMoniker.Reset();

                Marshal.ThrowExceptionForHR(CreateBindCtx(0, out bindCtx));

                while (enumMoniker.Next(1, monikers, IntPtr.Zero) == 0)
                {
                    string displayName;
                    monikers[0].GetDisplayName(bindCtx, null, out displayName);

                    if (displayName.StartsWith(visualStudioProgId))
                    {
                        object o;
                        Marshal.ThrowExceptionForHR(runningObjectTable.GetObject(monikers[0], out o));

                        var d = (DTE)o;

                        if (d.Solution.FullName.EndsWith(solution, StringComparison.InvariantCultureIgnoreCase)) return d;
                    }
                }
            }
            finally
            {
                if (runningObjectTable != null)
                {
                    Marshal.ReleaseComObject(runningObjectTable);
                }

                if (enumMoniker != null)
                {
                    Marshal.ReleaseComObject(enumMoniker);
                }

                if (bindCtx != null)
                {
                    Marshal.ReleaseComObject(bindCtx);
                }
            }

            return null;
        }
    }
}
