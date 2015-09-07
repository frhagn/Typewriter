using System;
using Typewriter.TemplateEditor.Lexing.Tokens;

namespace Typewriter.TemplateEditor.Lexing.Braces
{
    public class Brace
    {
        public Brace()
        {
        }

        public Brace(Token token, bool scopeChanged)
        {
            Token = token;
            ScopeChanged = scopeChanged;
        }

        public Token Token { get; set; }
        public bool ScopeChanged { get; set; }
    }
}