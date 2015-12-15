using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace Typewriter.VisualStudio
{
    [Guid(Constants.LanguageServiceId)]
    internal class LanguageService : IVsLanguageInfo, IVsLanguageTextOps
    {
        [Export, Name(Constants.ContentType), BaseDefinition(Constants.BaseDefinition)]
        internal static ContentTypeDefinition TstContentTypeDefinition { get; set; }

        [Export, ContentType(Constants.ContentType), FileExtension(Constants.TemplateExtension)]
        internal static FileExtensionToContentTypeDefinition TstFileExtensionDefinition { get; set; }

        [Export, Name(Classifications.Property)]
        internal static ClassificationTypeDefinition PropertyClassificationType { get; set; }

        [Export, Name(Classifications.AlternalteProperty)]
        internal static ClassificationTypeDefinition AlternatePropertyClassificationType { get; set; }

        [Export, Name(Classifications.ClassSymbol)]
        internal static ClassificationTypeDefinition ClassSymbolClassificationType { get; set; }

        [Export, Name(Classifications.InterfaceSymbol)]
        internal static ClassificationTypeDefinition InterfaceSymbolClassificationType { get; set; }

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
            pbstrExtensions = Constants.TemplateExtension;
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
