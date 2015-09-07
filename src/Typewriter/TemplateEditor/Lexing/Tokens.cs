using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;

namespace Typewriter.TemplateEditor.Lexing
{
    public class Tokens
    {
        private readonly BraceStack braces = new BraceStack();
        private readonly Dictionary<int, Token> tokenDictionary = new Dictionary<int, Token>();

        public BraceStack BraceStack => braces;

        public void Add(Token token)
        {
            tokenDictionary[token.Start] = token;
        }

        public void AddRange(IEnumerable<Token> tokens)
        {
            foreach (var token in tokens)
            {
                Add(token);
            }
        }
        
        public void Add(string classification, int start, int length = 1, string quickInfo = null)
        {
            Add(new Token { Start = start, Length = length, Classification = classification, QuickInfo = quickInfo });
        }

        internal void AddBrace(Stream stream, string classification = Classifications.Operator)
        {
            var brace = stream.Current;

            if (brace == '{' || brace == '[' || brace == '(' || brace == ')' || brace == ']' || brace == '}')
            {
                var token = new Token { Classification = classification, Start = stream.Position, Length = 1 };

                if (brace == '{' || brace == '[' || brace == '(')
                {
                    token.IsOpen = true;
                    braces.Push(token, stream.Current);
                }
                else
                {
                    var match = braces.Pop(brace);
                    if (match != null)
                    {
                        token.MatchingToken = match;
                        match.MatchingToken = token;
                    }
                }

                Add(token);
            }
        }

        public Token GetToken(int position)
        {
            return tokenDictionary.ContainsKey(position) ? tokenDictionary[position] : null;
        }

        public IEnumerable<Token> FindTokens(int position)
        {
            return tokenDictionary.Values.Where(t => t.Start <= position && t.Start + t.Length >= position);
        }

        public IEnumerable<Token> GetTokens(Span span)
        {
            return tokenDictionary.Where(t => t.Key >= span.Start && t.Key <= span.End).Select(t => t.Value);
        }
    }
}