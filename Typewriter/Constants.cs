using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Typewriter
{
    internal static class Constants
    {
        internal const string Extension = ".tst";
        internal const string ContentType = "tst";
        internal const string LanguageName = "TST";

        internal const string ExtensionPackageId = "45b6b392-ce2f-409c-a39f-bbf90b34349e";
        internal const string LanguageServiceId = "aa5d6809-9c5d-443c-a37c-c29e6af2fe15";

        internal const string BaseDefinition = "code";
        internal const char NewLine = '\n';
        
        public static readonly char[] Operators = "!&|+-/*?=,.:;<>%".ToCharArray();
        public static readonly string[] Keywords = 
        {
            "any",
            "boolean",
            "break",
            "case",
            "catch",
            "class",
            "const",
            "constructor",
            "continue",
            "declare",
            "do",
            "else",
            "enum",
            "export",
            "extends",
            "delete",
            "debugger",
            "default",
            "false",
            "finally",
            "for",
            "function",
            "get",
            "if",
            "implements",
            "import",
            "in",
            "instanceof",
            "interface",
            "let",
            "module",
            "new",
            "null",
            "number",
            "private",
            "protected",
            "public",
            "require",
            "return",
            "set",
            "static",
            "string",
            "super",
            "switch",
            "this",
            "throw",
            "true",
            "try",
            "typeof",
            "var",
            "void",
            "while",
            "with",
            "yield"
        };
    }

    public static class Classifications
    {
        public const string BraceHighlight = "MarkerFormatDefinition/HighlightedReference";
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
        internal static ContentTypeDefinition TstContentTypeDefinition { get; set; }

        [Export, ContentType(Constants.ContentType), FileExtension(Constants.Extension)]
        internal static FileExtensionToContentTypeDefinition TstFileExtensionDefinition { get; set; }

        [Export, Name(Classifications.Property)]
        internal static ClassificationTypeDefinition PropertyClassificationType { get; set; }

        // ReSharper restore UnassignedField.Compiler
    }
}
