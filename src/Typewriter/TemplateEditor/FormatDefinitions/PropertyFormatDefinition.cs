using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Typewriter.VisualStudio;

namespace Typewriter.TemplateEditor.FormatDefinitions
{
    [Export(typeof (EditorFormatDefinition)), Name("TypeScript Template Property")]
    [ClassificationType(ClassificationTypeNames = Classifications.Property)]
    internal sealed class PropertyFormatDefinition : ClassificationFormatDefinition
    {
        private static readonly Color _light = Color.FromRgb(188, 54, 3);
        private static readonly Color _dark = Color.FromRgb(204, 120, 50);

        public PropertyFormatDefinition()
        {
            this.ForegroundColor = ThemeInfo.IsDark ? _dark : _light;
        }
    }
}