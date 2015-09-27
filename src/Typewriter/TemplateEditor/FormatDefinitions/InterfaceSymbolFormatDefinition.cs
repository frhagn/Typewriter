using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Typewriter.VisualStudio;

namespace Typewriter.TemplateEditor.FormatDefinitions
{
    [Export(typeof(EditorFormatDefinition)), Name("TypeScript Template Interface Symbol")]
    [ClassificationType(ClassificationTypeNames = Classifications.InterfaceSymbol)]
    internal sealed class InterfaceSymbolFormatDefinition : ClassificationFormatDefinition
    {
        private static readonly Color light = Color.FromRgb(43, 145, 175); // Color.FromRgb(203, 75, 22) Color.FromRgb(59, 139, 59)
        private static readonly Color dark = Color.FromRgb(184, 215, 163); // Color.FromRgb(236, 118, 0) Color.FromRgb(103, 197, 103)

        public InterfaceSymbolFormatDefinition()
        {
            this.ForegroundColor = ThemeInfo.IsDark ? dark : light;
        }
    }
}
