using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using EnvDTE;

namespace Typewriter.Tests.TestInfrastructure
{
    internal static class Dte
    {
        [DllImport("ole32.dll")]
        private static extern int GetRunningObjectTable(uint dwReserved, out IRunningObjectTable pprot);

        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(uint dwReserved, out IBindCtx ppbc);

        internal static DTE GetInstance(string solution)
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