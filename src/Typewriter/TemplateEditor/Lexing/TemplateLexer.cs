using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Typewriter.CodeModel;

namespace Typewriter.TemplateEditor.Lexing
{
    public class TemplateLexer
    {
        private static readonly char[] _operators = "!&|+-/*?=,.:;<>%".ToCharArray();
        private static readonly string[] _symbolKeywords = { "as", "class", "extends", "implements", "import", "instanceof", "interface", "module", "new", "namespace", "typeof" };
        private static readonly string[] _keywords =
        {
            "abstract", "any", "as", "async", "await", "boolean", "break", "case", "catch", "class", "const", "constructor", "continue", "declare", "default",
            "do", "else", "enum", "export", "extends", "delete", "debugger", "default", "false", "finally", "for", "from",
            "function", "get", "if", "implements", "import", "in", "instanceof", "interface", "keyof", "let", "module", "namespace", "new",
            "null", "number", "of", "private", "protected", "public", "readonly", "require", "return", "set", "static", "string",
            "super", "switch", "this", "throw", "true", "try", "type", "typeof", "var", "void", "while", "with", "yield"
        };

        private readonly Contexts _contexts;
        private readonly Context _fileContext;

        private bool _isSymbol;
        private int _objectLiteralEnds;

        public TemplateLexer(Contexts contexts)
        {
            _contexts = contexts;
            _fileContext = contexts.Find(nameof(File));
        }

        public void Tokenize(SemanticModel semanticModel, string code)
        {
            _objectLiteralEnds = 0;
            _isSymbol = false;

            var context = new Stack<Context>(new[] { _fileContext });
            Parse(code, semanticModel, context, 0, 0);

            if (semanticModel.Tokens.BraceStack.IsBalanced(']') == false)
            {
                semanticModel.ErrorTokens.Add(new Token
                {
                    QuickInfo = "] expected",
                    Start = code.Length,
                    Length = 0
                });
            }
        }

        private void Parse(string template, SemanticModel semanticModel, Stack<Context> context, int offset, int depth)
        {
            var stream = new Stream(template, offset);

            do
            {
                if (ParseCodeBlock(stream, semanticModel, context)) continue;
                if (ParseDollar(stream, semanticModel, context, depth)) continue;
                if (ParseString(stream, semanticModel, context, depth)) continue;
                if (ParseComment(stream, semanticModel, context, depth)) continue;
                if (ParseNumber(stream, semanticModel)) continue;
                if (ParseOperators(stream, semanticModel)) continue;
                if (ParseKeywords(stream, semanticModel)) continue;
                if (ParseReference(stream, semanticModel)) continue;
                if (ParseSymbols(stream, semanticModel)) continue;

                TerminateSymbol(stream);
                
                semanticModel.Tokens.AddBrace(stream);
            }
            while (stream.Advance());

            var parent = context.Skip(1).FirstOrDefault();
            semanticModel.ContextSpans.Add(context.Peek(), parent, ContextType.Template, offset, stream.Position);
        }

        private bool ParseCodeBlock(Stream stream, SemanticModel semanticModel, Stack<Context> context)
        {
            if (stream.Current == '$' && stream.Peek() == '{' && context.Peek() == _fileContext)
            {
                for (var i = 0; ; i--)
                {
                    var current = stream.Peek(i);
                    if (current == '`' || (current == '/' && stream.Peek(i - 1) == '/')) return false;
                    if (current == '\n' || current == char.MinValue) break;
                }

                semanticModel.Tokens.Add(Classifications.Property, stream.Position);

                stream.Advance();
                semanticModel.Tokens.AddBrace(stream, Classifications.Property);

                var block = stream.PeekBlock(1, '{', '}');
                stream.Advance(block.Length);

                if (stream.Peek() == '}')
                {
                    stream.Advance();
                    semanticModel.Tokens.AddBrace(stream, Classifications.Property);
                }

                return true;
            }

            return false;
        }

        private bool ParseDollar(Stream stream, SemanticModel semanticModel, Stack<Context> context, int depth)
        {
            if (stream.Current == '$')
            {
                var identifier = GetIdentifier(stream, semanticModel, context);

                if (identifier != null)
                {
                    var classification = GetPropertyClassification(depth);

                    if (identifier.IsParent)
                    {
                        var parent = context.Skip(1).FirstOrDefault()?.Name.ToLowerInvariant();
                        semanticModel.Tokens.Add(classification, stream.Position, identifier.Name.Length + 1, identifier.QuickInfo.Replace("$parent", parent));
                        stream.Advance(identifier.Name.Length);

                        var current = context.Pop();

                        //ParseDot(stream, semanticModel, context, depth); // identifier
                        ParseBlock(stream, semanticModel, context, depth); // template

                        context.Push(current);
                    }
                    else
                    {
                        semanticModel.Tokens.Add(classification, stream.Position, identifier.Name.Length + 1, identifier.QuickInfo);
                        stream.Advance(identifier.Name.Length);

                        if (identifier.IsCollection)
                        {
                            context.Push(_contexts.Find(identifier.Context));

                            ParseFilter(stream, semanticModel, context, depth);
                            ParseBlock(stream, semanticModel, context, depth); // template

                            context.Pop();

                            ParseBlock(stream, semanticModel, context, depth); // separator
                        }
                        else if (identifier.IsBoolean)
                        {
                            ParseBlock(stream, semanticModel, context, depth); // true
                            ParseBlock(stream, semanticModel, context, depth); // false
                        }
                        else if (identifier.HasContext)
                        {
                            context.Push(_contexts.Find(identifier.Context));

                            //ParseDot(stream, semanticModel, context, depth); // identifier
                            ParseBlock(stream, semanticModel, context, depth); // template

                            context.Pop();
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        private void ParseFilter(Stream stream, SemanticModel semanticModel, Stack<Context> context, int depth)
        {
            if (stream.Peek() == '(')
            {
                stream.Advance();
                var classification = GetPropertyClassification(depth);

                semanticModel.Tokens.AddBrace(stream, classification);

                var block = stream.PeekBlock(1, '(', ')');

                if (block.Contains("=>") == false)
                    semanticModel.Tokens.Add(Classifications.String, stream.Position + 1, block.Length);

                stream.Advance(block.Length);

                if (stream.Peek() == ')')
                {
                    stream.Advance();
                    semanticModel.Tokens.AddBrace(stream, classification);
                }
            }
        }

        //private void ParseDot(Stream stream, SemanticModel semanticModel, Stack<Context> context, int depth)
        //{
        //    if (stream.Peek() == '.')
        //    {
        //        stream.Advance();
        //        var identifier = GetIdentifier(stream, semanticModel, context);
                
        //        if (identifier != null)
        //        {
        //            var classification = GetPropertyClassification(depth);
        //            var parent = context.Skip(1).FirstOrDefault();

        //            semanticModel.Tokens.Add(classification, stream.Position);
        //            semanticModel.ContextSpans.Add(context.Peek(), parent, ContextType.Template, stream.Position, stream.Position + 1);

        //            if (identifier.IsParent)
        //            {
        //                semanticModel.Tokens.Add(classification, stream.Position + 1, identifier.Name.Length, identifier.QuickInfo.Replace("$parent", parent?.Name.ToLowerInvariant()));
        //                stream.Advance(identifier.Name.Length);
                        
        //                var current = context.Pop();

        //                ParseDot(stream, semanticModel, context, depth); // identifier
        //                ParseBlock(stream, semanticModel, context, depth); // template

        //                context.Push(current);
        //            }
        //            else
        //            {
        //                semanticModel.Tokens.Add(classification, stream.Position + 1, identifier.Name.Length, identifier.QuickInfo);
        //                stream.Advance(identifier.Name.Length);

        //                if (identifier.IsCollection)
        //                {
        //                    context.Push(_contexts.Find(identifier.Context));

        //                    ParseFilter(stream, semanticModel, context, depth);
        //                    ParseBlock(stream, semanticModel, context, depth); // template

        //                    context.Pop();

        //                    ParseBlock(stream, semanticModel, context, depth); // separator
        //                }
        //                else if (identifier.IsBoolean)
        //                {
        //                    ParseBlock(stream, semanticModel, context, depth); // true
        //                    ParseBlock(stream, semanticModel, context, depth); // false
        //                }
        //                else if (identifier.HasContext)
        //                {
        //                    context.Push(_contexts.Find(identifier.Context));

        //                    ParseDot(stream, semanticModel, context, depth); // identifier
        //                    ParseBlock(stream, semanticModel, context, depth); // template

        //                    context.Pop();
        //                }
        //            }
        //        }
        //    }
        //}

        private void ParseBlock(Stream stream, SemanticModel semanticModel, Stack<Context> context, int depth)
        {
            if (stream.Peek() == '[')
            {
                stream.Advance();
                var classification = GetPropertyClassification(depth);

                semanticModel.Tokens.AddBrace(stream, classification);

                var block = stream.PeekBlock(1, '[', ']');
                Parse(block, semanticModel, context, stream.Position + 1, depth + 1);
                stream.Advance(block.Length);

                if (stream.Peek() == ']')
                {
                    stream.Advance();
                    semanticModel.Tokens.AddBrace(stream, classification);
                }
            }
        }

        private bool ParseString(Stream stream, SemanticModel semanticModel, Stack<Context> context, int depth)
        {
            if (stream.Current == '\'' || stream.Current == '"' || stream.Current == '`')
            {
                var start = stream.Position;
                var open = stream.Current;

                while (stream.Advance())
                {
                    var length = stream.Position - start;
                    if (ParseDollar(stream, semanticModel, context, depth))
                    {
                        semanticModel.Tokens.Add(Classifications.String, start, length);
                        if (stream.Advance() == false || stream.Current == Constants.NewLine) return true;
                        start = stream.Position;
                    }

                    if (stream.Current == open)
                    {
                        if (stream.Peek(-1) != '\\' || stream.Peek(-2) == '\\')
                        {
                            semanticModel.Tokens.Add(Classifications.String, start, stream.Position + 1 - start);
                            return true;
                        }
                    }
                }

                semanticModel.Tokens.Add(Classifications.String, start, stream.Position - start);
                return true;
            }

            return false;
        }

        private bool ParseComment(Stream stream, SemanticModel semanticModel, Stack<Context> context, int depth)
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
                        if (ParseDollar(stream, semanticModel, context, depth))
                        {
                            if (length > 0)
                                semanticModel.Tokens.Add(Classifications.Comment, start, length);
                            //if (stream.Advance() == false || stream.Current == Constants.NewLine) return true;
                            if (stream.Peek() == char.MinValue) return true;
                            start = stream.Position + 1;
                        }
                        if (stream.Current == Constants.NewLine) break;
                    }

                    semanticModel.Tokens.Add(Classifications.Comment, start, stream.Position - start);
                    return true;
                }

                if (type == '*')
                {
                    while (stream.Advance())
                    {
                        var length = stream.Position - start;

                        if (ParseDollar(stream, semanticModel, context, depth))
                        {
                            if (length > 0)
                                semanticModel.Tokens.Add(Classifications.Comment, start, length);
                            //if (stream.Advance() == false || stream.Current == Constants.NewLine) return true;
                            if (stream.Peek() == char.MinValue) return true;
                            start = stream.Position + 1;
                        }

                        if (stream.Current == Constants.NewLine)
                        {
                            semanticModel.Tokens.Add(Classifications.Comment, start, length);
                            if (stream.Advance(2) == false) return true;
                            start = stream.Position;
                        }

                        if (stream.Current == '*' && stream.Peek() == '/')
                        {
                            stream.Advance();
                            semanticModel.Tokens.Add(Classifications.Comment, start, stream.Position + 1 - start);
                            return true;
                        }
                    }

                    semanticModel.Tokens.Add(Classifications.Comment, start, stream.Position - start);
                    return true;
                }
            }

            return false;
        }

        private bool ParseNumber(Stream stream, SemanticModel semanticModel)
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

                semanticModel.Tokens.Add(Classifications.Number, start, stream.Position + 1 - start);
                return true;
            }

            return false;
        }

        private bool ParseOperators(Stream stream, SemanticModel semanticModel)
        {
            if (_operators.Contains(stream.Current))
            {
                semanticModel.Tokens.Add(Classifications.Operator, stream.Position);

                if (stream.Current == ':')
                {
                    while (stream.Peek() == ' ') stream.Advance();
                    _isSymbol = true;
                }
                return true;
            }

            return false;
        }

        private bool ParseKeywords(Stream stream, SemanticModel semanticModel)
        {
            var peek = stream.Peek(-1);

            if (char.IsLetterOrDigit(peek) == false && peek != '.')
            {
                var name = stream.PeekWord();

                if (name != null && _keywords.Contains(name))
                {
                    semanticModel.Tokens.Add(Classifications.Keyword, stream.Position, name.Length);
                    stream.Advance(name.Length - 1);

                    if (_symbolKeywords.Contains(name))
                    {
                        while (stream.Peek() == ' ') stream.Advance();
                        _isSymbol = true;
                    }

                    return true;
                }
            }

            return false;
        }

        private bool ParseReference(Stream stream, SemanticModel semanticModel)
        {
            const string keyword = "reference";

            if (stream.Current == '#' && stream.Peek() == keyword[0] && stream.PeekWord(1) == keyword)
            {
                semanticModel.Tokens.Add(Classifications.Directive, stream.Position, keyword.Length + 1);
                stream.Advance(keyword.Length);

                return true;
            }

            return false;
        }

        private bool ParseSymbols(Stream stream, SemanticModel semanticModel)
        {
            if (_isSymbol && stream.Position > _objectLiteralEnds)
            {
                var name = stream.PeekWord();
                if (name == null) return false;

                semanticModel.Tokens.Add(Classifications.ClassSymbol, stream.Position, name.Length);
                stream.Advance(name.Length - 1);
                
                return true;
            }

            return false;
        }

        private void TerminateSymbol(Stream stream)
        {
            if (stream.Current == ' ' || stream.Current == '{' || stream.Current == ';' || stream.Current == '(' || stream.Current == ')' || stream.Current == ',')
            {
                _isSymbol = false;
            }

            // Find object literals
            if (stream.Current == '{')
            {
                var i = 0;
                while (true) if (stream.Peek(--i) != ' ') break;

                var peek = stream.Peek(i);

                if (peek == '=' || peek == '(' || peek == ',')
                {
                    var block = stream.PeekBlock(1, '{', '}');
                    _objectLiteralEnds = Math.Max(_objectLiteralEnds, stream.Position + block.Length);
                }
            }
        }

        private Identifier GetIdentifier(Stream stream, SemanticModel semanticModel, Stack<Context> context)
        {
            var word = stream.PeekWord(1);
            if (word == null) return null;

            var c = context.Peek();
            var identifier = semanticModel.GetIdentifier(c, word);

            if (identifier == null)
                return null;

            if (identifier.IsBoolean == false && identifier.IsCollection == false)
                return identifier;

            var next = stream.Peek(identifier.Name.Length + 1);

            if (identifier.IsBoolean && next == '[')
                return identifier;

            if (identifier.IsCollection && (identifier.RequireTemplate == false || next == '[' || (identifier.IsCustom == false && next == '(')))
                return identifier;

            return null;
        }

        private static string GetPropertyClassification(int depth)
        {
            return depth % 2 == 0 ? Classifications.Property : Classifications.AlternalteProperty;
        }
    }
}
