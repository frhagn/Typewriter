using System;
using System.Collections.Generic;
using System.Linq;

namespace Typewriter.TemplateEditor.Lexing
{
    public class BraceStack
    {
        private readonly Stack<Token> braceStack = new Stack<Token>();
        private readonly Stack<Token> curlyBraceStack = new Stack<Token>();
        private readonly Stack<Token> functionBraceStack = new Stack<Token>();

        public void Push(Token token, char brace)
        {
            if (brace == '{') curlyBraceStack.Push(token);
            else if (brace == '[') braceStack.Push(token);
            else if (brace == '(') functionBraceStack.Push(token);
        }

        public Token Pop(char brace)
        {
            if (brace == '}' && curlyBraceStack.Count > 0) return curlyBraceStack.Pop();
            if (brace == ']' && braceStack.Count > 0) return braceStack.Pop();
            if (brace == ')' && functionBraceStack.Count > 0) return functionBraceStack.Pop();

            return null;
        }

        public bool IsBalanced(char brace)
        {
            if (brace == '}') return curlyBraceStack.Any() == false;
            if (brace == ']') return braceStack.Any() == false;
            if (brace == ')') return functionBraceStack.Any() == false;

            return false;
        }
    }
}
