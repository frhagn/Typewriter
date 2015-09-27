using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Typewriter.VisualStudio;

namespace Typewriter.TemplateEditor.FormatDefinitions
{
    [Export(typeof (EditorFormatDefinition)), Name("TypeScript Template Alternate Property")]
    [ClassificationType(ClassificationTypeNames = Classifications.AlternalteProperty)]
    internal sealed class AlternatePropertyFormatDefinition : ClassificationFormatDefinition
    {
        private static readonly Color _light = Color.FromRgb(228, 94, 43);
        private static readonly Color _dark = Color.FromRgb(244, 160, 90);

        public AlternatePropertyFormatDefinition()
        {
            this.ForegroundColor = ThemeInfo.IsDark ? _dark : _light;
        }
    }
}