using System;
using System.IO;
using Microsoft.Win32;

namespace Typewriter.VisualStudio
{
    internal static class IconRegistration
    {
        private static readonly string folder = GetFolder();

        public static void RegisterIcons()
        {
            try
            {
                using (RegistryKey classes = Registry.CurrentUser.OpenSubKey("SoftWare\\Classes", true))
                {
                    if (classes == null)
                        return;

                    AddIcon(classes, "tst.ico", ".tst");
                }
            }
            catch
            {
            }
        }

        private static void AddIcon(RegistryKey classes, string iconName, params string[] extensions)
        {
            foreach (var extension in extensions)
            {
                using (var key = classes.CreateSubKey(extension + "\\DefaultIcon"))
                {
                    if (key != null) key.SetValue(string.Empty, folder + iconName);
                }
            }
        }

        private static string GetFolder()
        {
            var directory = Path.GetDirectoryName(typeof(IconRegistration).Assembly.Location);
            return Path.Combine(directory, "VisualStudio\\Resources\\");
        }
    }
}
