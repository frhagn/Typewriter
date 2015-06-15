using System;
using System.Linq;

namespace Typewriter.TemplateEditor.Lexing
{
    public class Lexer
    {
        public Tokens Tokenize(string code)
        {
            var tokens = new Tokens();
            var context = Contexts.Find("File");

            Contexts.ClearTempIdentifiers();

            Parse(code, tokens, context, 0, 0);

            return tokens;
        }

        private void Parse(string template, Tokens tokens, Context context, int offset, int lineOffset)
        {
            var stream = new Stream(template, offset, lineOffset);
            var braces = new BraceStack();

            if (offset == 0) ParseCodeBlock(stream, tokens);

            do
            {
                if (ParseDollar(stream, tokens, context, braces)) continue;
                if (ParseString(stream, tokens, context, braces)) continue;
                if (ParseComment(stream, tokens, context, braces)) continue;
                if (ParseNumber(stream, tokens)) continue;
                if (ParseOperators(stream, tokens)) continue;
                if (ParseKeywords(stream, tokens)) continue;

                tokens.AddBrace(braces, stream);
            }
            while (stream.Advance());

            tokens.AddContext(context, offset, stream.Position);
        }

        private void ParseCodeBlock(Stream stream, Tokens tokens)
        {
            stream.SkipWhitespace();

            if (stream.Current == '$')
            {
                if (stream.Peek() == '{')
                {
                    var braces = new BraceStack();

                    tokens.AddToken(Classifications.Property, stream.Line, stream.Position);
                    stream.Advance();
                    tokens.AddBrace(braces, stream, Classifications.Property);

                    var block = stream.PeekBlock(1, '{', '}');

                    var code = new Stream(block, stream.Position + 1, stream.Line);

                    do
                    {
                        if (code.PeekWord() == "var")
                        {
                            tokens.AddToken(Classifications.Keyword, code.Line, code.Position, 3);
                            code.Advance(3);
                            code.SkipWhitespace();

                            var name = code.PeekWord();
                            if (name == null) continue;

                            code.Advance(name.Length);
                            code.SkipWhitespace();

                            if (code.Current != '=') continue;
                            code.Advance();

                            code.SkipWhitespace();
                            
                            if (code.Current != '(') continue;
                            tokens.AddBrace(braces, code);
                            code.Advance();

                            code.SkipWhitespace();

                            var type = code.PeekWord();
                            if (type == null) continue;

                            var context = Contexts.Find(type);
                            if(context == null) continue;

                            tokens.AddToken(Classifications.Property, code.Line, code.Position, type.Length);
                            code.Advance(type.Length);

                            context.AddTempIdentifier(name, "(extension) " + name);

                            do
                            {
                                if (code.Current == ')')
                                {
                                    tokens.AddBrace(braces, code);
                                    break;
                                }
                            } while (code.Advance());

                            code.Advance();
                            code.SkipWhitespace();

                            if (code.Current != '=' && code.Peek() != '>') continue;
                            code.Advance(2);

                            code.SkipWhitespace();

                            if (code.Current == '{')
                            {
                                tokens.AddBrace(braces, code);
                                var body = code.PeekBlock(1, '{', '}');
                                code.Advance(body.Length+1);
                                tokens.AddBrace(braces, code);
                            }
                            else
                            {
                                do
                                {
                                    if (code.Current == ';') break;
                                } while (code.Advance());
                            }
                        }
                    } while (code.Advance());

                    stream.Advance(block.Length);
                    
                    if (stream.Peek() == '}')
                    {
                        stream.Advance();
                        tokens.AddBrace(braces, stream, Classifications.Property);
                    }
                }
                else
                {
                    Contexts.Find("File").AddTempIdentifier("{");
                }
            }
        }

        private bool ParseDollar(Stream stream, Tokens tokens, Context context, BraceStack braces)
        {
            if (stream.Current == '$')
            {
                var word = stream.PeekWord(1);
                var identifier = context.GetIdentifier(word);

                if (identifier != null)
                {
                    if (IsValidIdentifier(stream, identifier))
                    {
                        tokens.AddToken(Classifications.Property, stream.Line, stream.Position, word.Length + 1, identifier.QuickInfo);
                        stream.Advance(word.Length);

                        if (identifier.IsCollection)
                        {
                            ParseFilter(stream, tokens, braces);
                            ParseBlock(stream, tokens, Contexts.Find(identifier.Context), braces); // template
                            ParseBlock(stream, tokens, context, braces); // separator
                        }
                        else if (identifier.IsBoolean)
                        {
                            ParseBlock(stream, tokens, context, braces); // true
                            ParseBlock(stream, tokens, context, braces); // false
                        }
                        else if (identifier.HasContext)
                        {
                            ParseBlock(stream, tokens, Contexts.Find(identifier.Context), braces); // template
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private void ParseFilter(Stream stream, Tokens tokens, BraceStack braces)
        {
            if (stream.Peek() == '(')
            {
                stream.Advance();
                tokens.AddBrace(braces, stream, Classifications.Property);

                var block = stream.PeekBlock(1, '(', ')');
                tokens.AddToken(Classifications.String, stream.Line, stream.Position + 1, block.Length);
                stream.Advance(block.Length);

                if (stream.Peek() == ')')
                {
                    stream.Advance();
                    tokens.AddBrace(braces, stream, Classifications.Property);
                }
            }
        }

        private void ParseBlock(Stream stream, Tokens tokens, Context context, BraceStack braces)
        {
            if (stream.Peek() == '[')
            {
                stream.Advance();
                tokens.AddBrace(braces, stream, Classifications.Property);

                var block = stream.PeekBlock(1, '[', ']');
                Parse(block, tokens, context, stream.Position + 1, stream.Line);
                stream.Advance(block.Length);

                if (stream.Peek() == ']')
                {
                    stream.Advance();
                    tokens.AddBrace(braces, stream, Classifications.Property);
                }
            }
        }

        private bool ParseString(Stream stream, Tokens tokens, Context context, BraceStack braces)
        {
            if (stream.Current == '\'' || stream.Current == '"')
            {
                var start = stream.Position;
                var open = stream.Current;

                while (stream.Advance())
                {
                    var length = stream.Position - start;
                    if (ParseDollar(stream, tokens, context, braces))
                    {
                        tokens.AddToken(Classifications.String, stream.Current == Constants.NewLine ? stream.Line - 1 : stream.Line, start, length);
                        if (stream.Advance() == false || stream.Current == Constants.NewLine) return true;
                        start = stream.Position;
                    }

                    if (stream.Current == open)
                    {
                        if (stream.Peek(-1) != '\\')
                        {
                            tokens.AddToken(Classifications.String, stream.Line, start, stream.Position + 1 - start);
                            return true;
                        }
                    }
                }

                tokens.AddToken(Classifications.String, stream.Line, start, stream.Position - start);
                return true;
            }

            return false;
        }

        private bool ParseComment(Stream stream, Tokens tokens, Context context, BraceStack braces)
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
                        if (ParseDollar(stream, tokens, context, braces))
                        {
                            tokens.AddToken(Classifications.Comment, stream.Current == Constants.NewLine ? stream.Line - 1 : stream.Line, start, length);
                            if (stream.Advance() == false || stream.Current == Constants.NewLine) return true;
                            start = stream.Position;
                        }
                        if (stream.Current == Constants.NewLine) break;
                    }

                    tokens.AddToken(Classifications.Comment, stream.Current == Constants.NewLine ? stream.Line - 1 : stream.Line, start, stream.Position - start);
                    return true;
                }

                if (type == '*')
                {
                    while (stream.Advance())
                    {
                        var length = stream.Position - start;

                        if (ParseDollar(stream, tokens, context, braces))
                        {
                            tokens.AddToken(Classifications.Comment, stream.Current == Constants.NewLine ? stream.Line - 1 : stream.Line, start, length);
                            if (stream.Advance() == false || stream.Current == Constants.NewLine) return true;
                            start = stream.Position;
                        }

                        if (stream.Current == Constants.NewLine)
                        {
                            tokens.AddToken(Classifications.Comment, stream.Line - 1, start, length);
                            if (stream.Advance(2) == false) return true;
                            start = stream.Position;
                        }

                        if (stream.Current == '*' && stream.Peek(1) == '/')
                        {
                            stream.Advance();
                            tokens.AddToken(Classifications.Comment, stream.Line, start, stream.Position + 1 - start);
                            return true;
                        }
                    }

                    tokens.AddToken(Classifications.Comment, stream.Current == Constants.NewLine ? stream.Line - 1 : stream.Line, start, stream.Position - start);
                    return true;
                }
            }

            return false;
        }

        private bool ParseNumber(Stream stream, Tokens tokens)
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

                tokens.AddToken(Classifications.Number, stream.Line, start, stream.Position + 1 - start);
                return true;
            }

            return false;
        }

        private bool ParseOperators(Stream stream, Tokens tokens)
        {
            if (Constants.Operators.Contains(stream.Current))
            {
                tokens.AddToken(Classifications.Operator, stream.Line, stream.Position);
                return true;
            }

            return false;
        }

        private bool ParseKeywords(Stream stream, Tokens tokens)
        {
            var name = stream.PeekWord();

            if (name == null) return false;

            if (Constants.Keywords.Contains(name))
            {
                tokens.AddToken(Classifications.Keyword, stream.Line, stream.Position, name.Length);
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