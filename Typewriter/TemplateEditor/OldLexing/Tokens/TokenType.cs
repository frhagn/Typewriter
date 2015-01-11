using System;

namespace Typewriter.TemplateEditor.Lexing.Tokens
{
    public enum TokenType
    {
        OpenBrace,
        CloseBrace,
        OpenCurlyBrace,
        CloseCurlyBrace,
        OpenFunctionBrace,
        CloseFunctionBrace,
        OpenBracket,
        CloseBracket,
        Keyword,
        Comment,
        Identifier,
        Filter
    }
}