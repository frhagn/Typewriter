using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Typewriter.TemplateEditor
{
    internal static class Constants
    {
        internal const string Extension = ".tst";
        internal const string ContentType = "tst";
        internal const string BaseDefinition = "plaintext";
    }

    public static class Classifications
    {
        public const string BraceMatching = "MarkerFormatDefinition/HighlightedReference";
        public const string Comment = "Comment";
        public const string Identifier = "Identifier";
        public const string Keyword = "Keyword";
        public const string Number = "Number";
        public const string Operator = "Operator";
        public const string Property = "Tst/Property";
        public const string String = "String";
        public const string SyntaxError = "syntax error";
    }

    internal static class Exports
    {
        // ReSharper disable UnassignedField.Compiler

        [Export, Name(Constants.ContentType), BaseDefinition(Constants.BaseDefinition)]
        internal static ContentTypeDefinition TstContentTypeDefinition;

        [Export, ContentType(Constants.ContentType), FileExtension(Constants.Extension)]
        internal static FileExtensionToContentTypeDefinition TstFileExtensionDefinition;

        // ReSharper restore UnassignedField.Compiler
    }
}
