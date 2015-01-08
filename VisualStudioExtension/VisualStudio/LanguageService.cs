using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TextManager.Interop;
using Typewriter.TemplateEditor;

namespace Typewriter.VisualStudio
{
    [GuidAttribute(Constants.LanguageServiceId)]
    class LanguageService : IVsLanguageInfo, IVsLanguageTextOps
    {
        private const int failed = 2147467263;

        public int GetCodeWindowManager(IVsCodeWindow pCodeWin, out IVsCodeWindowManager ppCodeWinMgr)
        {
            ppCodeWinMgr = null;
            return failed;
        }

        public int GetColorizer(IVsTextLines pBuffer, out IVsColorizer ppColorizer)
        {
            ppColorizer = null;
            return failed;
        }

        public int GetFileExtensions(out string pbstrExtensions)
        {
            pbstrExtensions = Constants.Extension;
            return 0;
        }

        public int GetLanguageName(out string bstrName)
        {
            bstrName = Constants.LanguageName;
            return 0;
        }

        public int Format(IVsTextLayer pTextLayer, TextSpan[] ptsSel)
        {
            return failed;
        }

        public int GetDataTip(IVsTextLayer pTextLayer, TextSpan[] ptsSel, TextSpan[] ptsTip, out string pbstrText)
        {
            pbstrText = null;
            return failed;
        }

        public int GetPairExtent(IVsTextLayer pTextLayer, TextAddress ta, TextSpan[] pts)
        {
            return failed;
        }

        public int GetWordExtent(IVsTextLayer pTextLayer, TextAddress ta, WORDEXTFLAGS flags, TextSpan[] pts)
        {
            return failed;
        }
    }
}
