using System;
using System.Collections.Generic;
using System.Linq;

namespace Typewriter.TemplateEditor.Lexing
{
    public class Lexer
    {
        private readonly Dictionary<int, Token> tokens = new Dictionary<int, Token>();
        private readonly Dictionary<int, ICollection<Token>> lines = new Dictionary<int, ICollection<Token>>();
        private readonly List<ContextSpan> contexts = new List<ContextSpan>();

        public IEnumerable<Token> _Tokens
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

        public Lexer(string code)
        {
            Parse(code, Contexts.Find("File"), 0, 0);
        }

        private Token AddToken(string classification, int line, int start, int length = 1, string quickInfo = null)
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

        private void AddBrace(BraceStack braces, Stream stream, string classification = Classifications.Operator)
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

        private void Parse(string code, Context context, int offset, int lineOffset)
        {
            var stream = new Stream(code, offset, lineOffset);
            var braces = new BraceStack();

            do
            {
                if (ParseDollar(stream, context, braces)) continue;
                if (ParseString(stream, context, braces)) continue;
                if (ParseComment(stream, context, braces)) continue;
                if (ParseNumber(stream)) continue;
                if (ParseOperators(stream)) continue;
                if (ParseKeywords(stream)) continue;

                AddBrace(braces, stream);
            }
            while (stream.Advance());

            contexts.Add(new ContextSpan(offset, stream.Position, context));
        }

        private bool ParseDollar(Stream stream, Context context, BraceStack braces)
        {
            if (stream.Current == '$')
            {
                var word = stream.PeekWord(1);
                var identifier = context.GetIdentifier(word);

                if (identifier != null)
                {
                    if (IsValidIdentifier(stream, identifier))
                    {
                        AddToken(Classifications.Property, stream.Line, stream.Position, word.Length + 1, identifier.QuickInfo);
                        stream.Advance(word.Length);

                        if (identifier.IsCollection)
                        {
                            ParseFilter(stream, braces);
                            ParseBlock(stream, Contexts.Find(identifier.Context), braces); // template
                            ParseBlock(stream, context, braces); // separator
                        }
                        else if (identifier.IsBoolean)
                        {
                            ParseBlock(stream, context, braces); // true
                            ParseBlock(stream, context, braces); // false
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private void ParseFilter(Stream stream, BraceStack braces)
        {
            if (stream.Peek() == '(')
            {
                stream.Advance();
                AddBrace(braces, stream, Classifications.Property);

                var block = stream.PeekBlock(1, '(', ')');
                AddToken(Classifications.String, stream.Line, stream.Position + 1, block.Length);
                stream.Advance(block.Length);

                if (stream.Peek() == ')')
                {
                    stream.Advance();
                    AddBrace(braces, stream, Classifications.Property);
                }
            }
        }

        private void ParseBlock(Stream stream, Context context, BraceStack braces)
        {
            if (stream.Peek() == '[')
            {
                stream.Advance();
                AddBrace(braces, stream, Classifications.Property);

                var block = stream.PeekBlock(1);
                Parse(block, context, stream.Position + 1, stream.Line);
                stream.Advance(block.Length);

                if (stream.Peek() == ']')
                {
                    stream.Advance();
                    AddBrace(braces, stream, Classifications.Property);
                }
            }
        }

        private bool ParseString(Stream stream, Context context, BraceStack braces)
        {
            if (stream.Current == '\'' || stream.Current == '"')
            {
                var start = stream.Position;
                var open = stream.Current;

                while (stream.Advance())
                {
                    var length = stream.Position - start;
                    if (ParseDollar(stream, context, braces))
                    {
                        AddToken(Classifications.String, stream.Current == '\r' ? stream.Line - 1 : stream.Line, start, length);
                        if (stream.Advance() == false || stream.Current == '\r') return true;
                        start = stream.Position;
                    }

                    if (stream.Current == open)
                    {
                        if (stream.Peek(-1) != '\\')
                        {
                            AddToken(Classifications.String, stream.Line, start, stream.Position + 1 - start);
                            return true;
                        }
                    }
                }

                AddToken(Classifications.String, stream.Line, start, stream.Position - start);
                return true;
            }

            return false;
        }

        private bool ParseComment(Stream stream, Context context, BraceStack braces)
        {
            if (stream.Current == '/')
            {
                var type = stream.Peek();
                var start = stream.Position;

                if (type == '/')
                {
                    while (stream.Advance())
                    {
                        var length = stream.Position - start;
                        if (ParseDollar(stream, context, braces))
                        {
                            AddToken(Classifications.Comment, stream.Current == '\r' ? stream.Line - 1 : stream.Line, start, length);
                            if (stream.Advance() == false || stream.Current == '\r') return true;
                            start = stream.Position;
                        }
                        if (stream.Current == '\r') break;
                    }

                    AddToken(Classifications.Comment, stream.Current == '\r' ? stream.Line - 1 : stream.Line, start, stream.Position - start);
                    return true;
                }

                if (type == '*')
                {
                    while (stream.Advance())
                    {
                        var length = stream.Position - start;

                        if (ParseDollar(stream, context, braces))
                        {
                            AddToken(Classifications.Comment, stream.Current == '\r' ? stream.Line - 1 : stream.Line, start, length);
                            if (stream.Advance() == false || stream.Current == '\r') return true;
                            start = stream.Position;
                        }

                        if (stream.Current == '\r')
                        {
                            AddToken(Classifications.Comment, stream.Line - 1, start, length);
                            if (stream.Advance(2) == false) return true;
                            start = stream.Position;
                        }

                        if (stream.Current == '*' && stream.Peek(1) == '/')
                        {
                            stream.Advance();
                            AddToken(Classifications.Comment, stream.Line, start, stream.Position + 1 - start);
                            return true;
                        }
                    }

                    AddToken(Classifications.Comment, stream.Current == '\r' ? stream.Line - 1 : stream.Line, start, stream.Position - start);
                    return true;
                }
            }

            return false;
        }

        private bool ParseNumber(Stream stream)
        {
            if (char.IsDigit(stream.Current) || (stream.Current == '.' && char.IsDigit(stream.Peek())))
            {
                var start = stream.Position;

                do
                {
                    if (char.IsDigit(stream.Peek()) == false && (stream.Peek() == '.' && char.IsDigit(stream.Peek(2))) == false)
                        break;
                }
                while (stream.Advance());

                AddToken(Classifications.Number, stream.Line, start, stream.Position + 1 - start);
                return true;
            }

            return false;
        }

        private bool ParseOperators(Stream stream)
        {
            if (Tokens.Operators.Contains(stream.Current))
            {
                AddToken(Classifications.Operator, stream.Line, stream.Position);
                return true;
            }

            return false;
        }

        private bool ParseKeywords(Stream stream)
        {
            var name = stream.PeekWord();

            if (name == null) return false;

            if (Tokens.Keywords.Contains(name))
            {
                AddToken(Classifications.Keyword, stream.Line, stream.Position, name.Length);
            }

            stream.Advance(name.Length - 1);
            return true;
        }

        private bool IsValidIdentifier(Stream stream, Identifier identifier)
        {
            if (identifier.IsBoolean == false && identifier.IsCollection == false)
                return true;

            var next = stream.Peek(identifier.Name.Length + 1);

            if (identifier.IsBoolean && next == '[')
                return true;

            if (identifier.IsCollection && (next == '[' || next == '('))
                return true;

            return false;
        }
    }
}