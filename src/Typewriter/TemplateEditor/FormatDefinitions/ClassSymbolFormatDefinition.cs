using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Typewriter.VisualStudio;

namespace Typewriter.TemplateEditor.FormatDefinitions
{
    [Export(typeof (EditorFormatDefinition)), Name("TypeScript Template Class Symbold")]
    [ClassificationType(ClassificationTypeNames = Classifications.ClassSymbol)]
    internal sealed class ClassSymbolFormatDefinition : ClassificationFormatDefinition
    {
        private static readonly Color _light = Color.FromRgb(43, 145, 175);
        private static readonly Color _dark = Color.FromRgb(78, 201, 176);

        public ClassSymbolFormatDefinition()
        {
            this.ForegroundColor = ThemeInfo.IsDark ? _dark : _light;
        }
    }
}