using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Typewriter.TemplateEditor.FormatDefinitions
{
    internal static class FormatDefinitionExports
    {
        // ReSharper disable UnassignedField.Compiler

        [Export, Name(Classifications.Property)]
        internal static ClassificationTypeDefinition PropertyClassificationType;

        // ReSharper restore UnassignedField.Compiler
    }
}