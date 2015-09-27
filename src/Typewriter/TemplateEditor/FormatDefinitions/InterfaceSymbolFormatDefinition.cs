using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Typewriter.VisualStudio;

namespace Typewriter.TemplateEditor.FormatDefinitions
{
    [Export(typeof (EditorFormatDefinition)), Name("TypeScript Template Interface Symbol")]
    [ClassificationType(ClassificationTypeNames = Classifications.InterfaceSymbol)]
    internal sealed class InterfaceSymbolFormatDefinition : ClassificationFormatDefinition
    {
        private static readonly Color _light = Color.FromRgb(43, 145, 175);
        private static readonly Color _dark = Color.FromRgb(184, 215, 163);

        public InterfaceSymbolFormatDefinition()
        {
            this.ForegroundColor = ThemeInfo.IsDark ? _dark : _light;
        }
    }
}