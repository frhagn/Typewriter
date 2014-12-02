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

        protected override FormatColorStorage Light { get { return new FormatColorStorage { Foreground = Color.FromRgb(59, 139, 59) }; } }
        protected override FormatColorStorage Dark { get { return new FormatColorStorage { Foreground = Color.FromRgb(103, 197, 103) }; } }
    }
}
