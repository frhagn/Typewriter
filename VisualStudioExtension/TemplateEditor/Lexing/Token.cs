using System;
using System.Collections.Generic;
using System.Linq;

namespace Typewriter.TemplateEditor.Lexing
{
    public class Token
    {
        public int Line { get; set; }

        public int Start { get; set; }
        public int Length { get; set; }
        public string Classification { get; set; }
        public string QuickInfo { get; set; }
        public Token MatchingToken { get; set; }
        public bool IsOpen { get; set; }
    }

    public class Tokens
    {
        private readonly Dictionary<int, Token> tokens = new Dictionary<int, Token>();
        private readonly Dictionary<int, ICollection<Token>> lines = new Dictionary<int, ICollection<Token>>();
        private readonly List<ContextSpan> contexts = new List<ContextSpan>();

        public IEnumerable<Token> List
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

        public Context GetContext(int position)
        {
            return contexts.Where(c => c.Start <= position && c.End > position).OrderByDescending(c => c.Start).First().Context;
        }

        internal Token AddToken(string classification, int line, int start, int length = 1, string quickInfo = null)
        {
            var token = new Token { Line = line, Start = start, Length = length, Classification = classification, QuickInfo = quickInfo };

            tokens[start] = token;

            if (lines.ContainsKey(line))
            {
                lines[line].Add(token);
            }
            else
            {
                lines[line] = new List<Token> { token };
            }

            return token;
        }

        internal void AddBrace(BraceStack braces, Stream stream, string classification = Classifications.Operator)
        {
            var brace = stream.Current;

            if (brace == '{' || brace == '[' || brace == '(' || brace == ')' || brace == ']' || brace == '}')
            {
                var token = AddToken(classification, stream.Line, stream.Position);

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
            }
        }

        internal void AddContext(Context context, int offset, int position)
        {
            contexts.Add(new ContextSpan(offset, position, context));
        }
    }
}