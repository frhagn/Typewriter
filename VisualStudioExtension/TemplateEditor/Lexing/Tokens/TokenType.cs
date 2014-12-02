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
        Keyword,
        Comment,
        Identifier,
        Filter,
        OpenBlock,
        CloseBlock,
    }
}