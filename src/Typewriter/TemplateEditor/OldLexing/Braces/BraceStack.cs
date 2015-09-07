using System;
using System.Collections.Generic;
using Typewriter.TemplateEditor.Lexing.Tokens;

namespace Typewriter.TemplateEditor.Lexing.Braces
{
    public class BraceStack
    {
        private readonly Stack<Brace> braceStack = new Stack<Brace>();
        private readonly Stack<Brace> curlyBraceStack = new Stack<Brace>();
        private readonly Stack<Brace> functionBraceStack = new Stack<Brace>();
        private readonly Stack<Brace> bracketStack = new Stack<Brace>();

        public void Push(Token token, bool scopeChanged)
        {
            switch (token.Type)
            {
                case TokenType.OpenBrace:
                    braceStack.Push(new Brace(token, scopeChanged));
                    break;
                case TokenType.OpenCurlyBrace:
                    curlyBraceStack.Push(new Brace(token, scopeChanged));
                    break;
                case TokenType.OpenFunctionBrace:
                    functionBraceStack.Push(new Brace(token, scopeChanged));
                    break;
                case TokenType.OpenBracket:
                    bracketStack.Push(new Brace(token, scopeChanged));
                    break;
            }
        }

        public Brace Pop(TokenType tokenType)
        {
            switch (tokenType)
            {
                case TokenType.OpenBrace:
                    if (braceStack.Count > 0)
                        return braceStack.Pop();
                    return new Brace();

                case TokenType.OpenCurlyBrace:
                    if (curlyBraceStack.Count > 0)
                        return curlyBraceStack.Pop();
                    return new Brace();

                case TokenType.OpenFunctionBrace:
                    if (functionBraceStack.Count > 0)
                        return functionBraceStack.Pop();
                    return new Brace();

                case TokenType.OpenBracket:
                    if (bracketStack.Count > 0)
                        return bracketStack.Pop();
                    return new Brace();
            }

            return null;
        }
    }
}