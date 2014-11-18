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

        public const string KeywordClassificationType = "tst.keywordType";
        public const string KeywordClassificationName = "TypeScript Template Keyword";
        public const string PropertyClassificationType = "tst.propertyType";
        public const string PropertyClassificationName = "TypeScript Template Property";
        public const string CommentClassificationType = "tst.commentType";
        public const string CommentClassificationName = "TypeScript Template Comment";
    }

    internal static class Exports
    {
        // ReSharper disable UnassignedField.Compiler

        [Export, Name(Constants.ContentType), BaseDefinition(Constants.BaseDefinition)]
        internal static ContentTypeDefinition TstContentTypeDefinition;

        [Export, ContentType(Constants.ContentType), FileExtension(Constants.Extension)]
        internal static FileExtensionToContentTypeDefinition TstFileExtensionDefinition;

        [Export, Name(Constants.KeywordClassificationType)] 
        internal static ClassificationTypeDefinition KeywordClassificationType;

        [Export, Name(Constants.PropertyClassificationType)]
        internal static ClassificationTypeDefinition PropertyClassificationType;

        [Export, Name(Constants.CommentClassificationType)]
        internal static ClassificationTypeDefinition CommentClassificationType;

        // ReSharper restore UnassignedField.Compiler
    }
}
