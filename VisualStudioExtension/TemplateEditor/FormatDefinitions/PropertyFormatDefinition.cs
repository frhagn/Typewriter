using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Typewriter.TemplateEditor.FormatDefinitions
{
    [Export(typeof(EditorFormatDefinition)), Name("TypeScript Template Property")]
    [ClassificationType(ClassificationTypeNames = Classifications.Property)]
    internal sealed class PropertyFormatDefinition : FormatDefinitionBase
    {
        public PropertyFormatDefinition() : base(true, false)
        {
            this.DisplayName = "TypeScript Template Property";
            this.ForegroundCustomizable = false;
            this.BackgroundCustomizable = false;
        }

        protected override ColorFormat Light { get { return new ColorFormat { Foreground = Color.FromRgb(43, 145, 175) }; } } // Color.FromRgb(203, 75, 22) Color.FromRgb(59, 139, 59)
        protected override ColorFormat Dark { get { return new ColorFormat { Foreground = Color.FromRgb(184, 215, 163) }; } } // Color.FromRgb(236, 118, 0) Color.FromRgb(103, 197, 103)
    }
}
