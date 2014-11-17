using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Typewriter.TemplateEditor
{
    [Export(typeof(EditorFormatDefinition)), Name(Constants.KeywordClassificationName)]
    [ClassificationType(ClassificationTypeNames = Constants.KeywordClassificationType)]
    internal sealed class KeywordClassificationFormat : ClassificationFormatDefinition
    {
        public KeywordClassificationFormat()
        {
            this.DisplayName = Constants.KeywordClassificationName;
            this.ForegroundColor = Colors.Blue;
        }
    }
    
    [Export(typeof(EditorFormatDefinition)), Name(Constants.PropertyClassificationName)]
    [ClassificationType(ClassificationTypeNames = Constants.PropertyClassificationType)]
    internal sealed class PropertyClassificationFormat : ClassificationFormatDefinition
    {
        public PropertyClassificationFormat()
        {
            this.DisplayName = Constants.PropertyClassificationName;
            this.ForegroundColor = Colors.Firebrick;
        }
    }
    
    [Export(typeof(EditorFormatDefinition)), Name(Constants.CommentClassificationName)]
    [ClassificationType(ClassificationTypeNames = Constants.CommentClassificationType)]
    internal sealed class CommentClassificationFormat : ClassificationFormatDefinition
    {
        public CommentClassificationFormat()
        {
            this.DisplayName = Constants.CommentClassificationName;
            this.ForegroundColor = Colors.Green;
        }
    }
}
