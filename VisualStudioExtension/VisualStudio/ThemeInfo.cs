using System;
using System.Drawing;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Typewriter.VisualStudio
{
    internal static class ThemeInfo
    {
        private static bool? isDark;

        internal static bool IsDark
        {
            get
            {
                if (isDark == null)
                {
                    isDark = GetIsDark();
                }

                return isDark.Value;
            }
        }

        private static bool GetIsDark()
        {
            var storage = Package.GetGlobalService(typeof(SVsFontAndColorStorage)) as IVsFontAndColorStorage;
            if (storage == null) return false;

            var category = Microsoft.VisualStudio.Editor.DefGuidList.guidTextEditorFontCategory;
            var success = storage.OpenCategory(ref category, (uint)(__FCSTORAGEFLAGS.FCSF_READONLY | __FCSTORAGEFLAGS.FCSF_LOADDEFAULTS | __FCSTORAGEFLAGS.FCSF_NOAUTOCOLORS));

            try
            {
                if (success == 0)
                {
                    var colors = new ColorableItemInfo[1];
                    var hresult = storage.GetItem("Plain Text", colors);
                    if (hresult == 0)
                    {
                        var dcolor = ColorTranslator.FromOle(Convert.ToInt32(colors[0].crBackground));
                        var hsp = Math.Sqrt(0.299 * (dcolor.R * dcolor.R) + 0.587 * (dcolor.G * dcolor.G) + 0.114 * (dcolor.B * dcolor.B));
                        return hsp < 127.5;
                    }
                }
            }
            finally
            {
                storage.CloseCategory();
            }

            return false;
        }
    }
}