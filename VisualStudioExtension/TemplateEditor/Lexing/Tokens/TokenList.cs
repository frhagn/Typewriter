using System;
using System.Collections.Generic;

namespace Typewriter.TemplateEditor.Lexing.Tokens
{
    public class TokenList //: List<Token>
    {
        private readonly Dictionary<int, Token> tokens = new Dictionary<int, Token>();
        private readonly Dictionary<int, ICollection<Token>> lines = new Dictionary<int, ICollection<Token>>();

        public Token Add(Token token)
        {
            tokens[token.Start] = token;

            if (lines.ContainsKey(token.Line))
            {
                lines[token.Line].Add(token);
            }
            else
            {
                lines[token.Line] = new List<Token> { token };
            }

            return token;
        }

        public Token GetToken(int position)
        {
            return tokens.ContainsKey(position) ? tokens[position] : null;
        }

        public IEnumerable<Token> GetTokensForLine(int line)
        {
            return lines.ContainsKey(line) ? lines[line] : new Token[0];
        }
    }
}