using System;
using System.Collections.Generic;
using System.Linq;
using Typewriter.TemplateEditor.Lexing.Contexts;

namespace Typewriter.TemplateEditor.Lexing.Tokens
{
    public class TokenList //: List<Token>
    {
        private readonly List<ContextSpan> contexts = new List<ContextSpan>(); 
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

        public void AddContext(ContextSpan context)
        {
            contexts.Add(context);
        }

        public IContext GetContext(int position)
        {
            return contexts.Where(c => c.Start <= position && c.End > position).OrderByDescending(c => c.Start).First().Context;
        }

        public IEnumerable<Token> Tokens
        {
            get { return tokens.Values; }
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