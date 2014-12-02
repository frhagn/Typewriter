using System;
using System.Collections.Generic;

namespace Typewriter.TemplateEditor.Lexing.Tokens
{
    public class TokenList : List<Token>
    {
        public new Token Add(Token token)
        {
            base.Add(token);
            return token;
        }
    }
}