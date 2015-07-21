using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Typewriter.CodeModel;
using Typewriter.TemplateEditor.Lexing.Roslyn;

namespace Typewriter.TemplateEditor.Lexing
{
    public class TemplateLexer
    {
        private static readonly Context fileContext = Contexts.Find(nameof(File));
        private static readonly char[] operators = "!&|+-/*?=,.:;<>%".ToCharArray();
        private static readonly string[] keywords =
        {
            "any", "boolean", "break", "case", "catch", "class", "const", "constructor", "continue", "declare",
            "do", "else", "enum", "export", "extends", "delete", "debugger", "default", "false", "finally", "for",
            "function", "get", "if", "implements", "import", "in", "instanceof", "interface", "let", "module", "new",
            "null", "number", "private", "protected", "public", "require", "return", "set", "static", "string",
            "super", "switch", "this", "throw", "true", "try", "typeof", "var", "void", "while", "with", "yield"
        };

        public void Tokenize(SemanticModel semanticModel, string code)
        {
            var context = new Stack<Context>(new[] { fileContext });
            Parse(code, semanticModel, context, 0);
        }

        private void Parse(string template, SemanticModel semanticModel, Stack<Context> context, int offset)
        {
            var stream = new Stream(template, offset);

            do
            {
                if (ParseCodeBlock(stream, semanticModel, context)) continue;
                if (ParseDollar(stream, semanticModel, context)) continue;
                if (ParseString(stream, semanticModel, context)) continue;
                if (ParseComment(stream, semanticModel, context)) continue;
                if (ParseNumber(stream, semanticModel)) continue;
                if (ParseOperators(stream, semanticModel)) continue;
                if (ParseKeywords(stream, semanticModel)) continue;

                semanticModel.Tokens.AddBrace(stream);
            }
            while (stream.Advance());

            var parent = context.Skip(1).FirstOrDefault();
            semanticModel.ContextSpans.Add(context.Peek(), parent, ContextType.Template, offset, stream.Position);
        }

        private bool ParseCodeBlock(Stream stream, SemanticModel semanticModel, Stack<Context> context)
        {
            if (stream.Current == '$' && stream.Peek() == '{' && context.Peek() == fileContext)
            {
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

        private bool ParseDollar(Stream stream, SemanticModel semanticModel, Stack<Context> context)
        {
            if (stream.Current == '$')
            {
                var identifier = GetIdentifier(stream, semanticModel, context);

                if (identifier != null)
                {
                    if (identifier.IsParent)
                    {
                        var parent = context.Skip(1).FirstOrDefault()?.Name.ToLowerInvariant();
                        semanticModel.Tokens.Add(Classifications.Property, stream.Position, identifier.Name.Length + 1, identifier.QuickInfo.Replace("$parent", parent));
                        stream.Advance(identifier.Name.Length);

                        var current = context.Pop();
                        ParseBlock(stream, semanticModel, context); // template
                        context.Push(current);
                    }
                    else
                    {
                        semanticModel.Tokens.Add(Classifications.Property, stream.Position, identifier.Name.Length + 1, identifier.QuickInfo);
                        stream.Advance(identifier.Name.Length);

                        if (identifier.IsCollection)
                        {
                            context.Push(Contexts.Find(identifier.Context));

                            ParseFilter(stream, semanticModel, context);
                            ParseBlock(stream, semanticModel, context); // template

                            context.Pop();

                            ParseBlock(stream, semanticModel, context); // separator
                        }
                        else if (identifier.IsBoolean)
                        {
                            ParseBlock(stream, semanticModel, context); // true
                            ParseBlock(stream, semanticModel, context); // false
                        }
                        else if (identifier.HasContext)
                        {
                            context.Push(Contexts.Find(identifier.Context));

                            //ParseDot(stream, SemanticModel, Contexts.Find(identifier.Context), context); // Identifier
                            ParseBlock(stream, semanticModel, context); // template

                            context.Pop();
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        private void ParseFilter(Stream stream, SemanticModel semanticModel, Stack<Context> context)
        {
            if (stream.Peek() == '(')
            {
                stream.Advance();
                semanticModel.Tokens.AddBrace(stream, Classifications.Property);

                var block = stream.PeekBlock(1, '(', ')');

                if(block.Contains("=>") == false)
                    semanticModel.Tokens.Add(Classifications.String, stream.Position + 1, block.Length);

                stream.Advance(block.Length);

                if (stream.Peek() == ')')
                {
                    stream.Advance();
                    semanticModel.Tokens.AddBrace(stream, Classifications.Property);
                }
            }
        }

        //private void ParseDot(Stream stream, SemanticModel SemanticModel, Context context, Context parentContext)
        //{
        //    if (stream.Peek() == '.')
        //    {
        //        stream.Advance();

        //        var word = stream.PeekWord(1);
        //        var identifier = context.GetIdentifier(word);

        //        SemanticModel.AddContext(context, stream.Position+1, stream.Position + 2);

        //        if (identifier != null)
        //        {
        //            if (IsValidIdentifier(stream, identifier))
        //            {
        //                SemanticModel.AddToken(Classifications.Property, stream.Line, stream.Position);
        //                SemanticModel.AddToken(Classifications.Property, stream.Line, stream.Position + 1, word.Length, identifier.QuickInfo);
        //                stream.Advance(word.Length);

        //                if (identifier.IsCollection)
        //                {
        //                    var subContext = Contexts.Find(identifier.Context);
        //                    ParseFilter(stream, SemanticModel, subContext, braces);
        //                    ParseBlock(stream, SemanticModel, subContext, braces); // template
        //                    ParseBlock(stream, SemanticModel, context, braces); // separator
        //                }
        //                else if (identifier.IsBoolean)
        //                {
        //                    ParseBlock(stream, SemanticModel, parentContext, braces); // true
        //                    ParseBlock(stream, SemanticModel, parentContext, braces); // false
        //                }
        //                else if (identifier.HasContext)
        //                {
        //                    ParseDot(stream, SemanticModel, Contexts.Find(identifier.Context), parentContext, braces); // Identifier
        //                    ParseBlock(stream, SemanticModel, Contexts.Find(identifier.Context), braces); // template
        //                }
        //            }
        //        }
        //    }
        //}

        private void ParseBlock(Stream stream, SemanticModel semanticModel, Stack<Context> context)
        {
            if (stream.Peek() == '[')
            {
                stream.Advance();
                semanticModel.Tokens.AddBrace(stream, Classifications.Property);

                var block = stream.PeekBlock(1, '[', ']');
                Parse(block, semanticModel, context, stream.Position + 1);
                stream.Advance(block.Length);

                if (stream.Peek() == ']')
                {
                    stream.Advance();
                    semanticModel.Tokens.AddBrace(stream, Classifications.Property);
                }
            }
        }

        private bool ParseString(Stream stream, SemanticModel semanticModel, Stack<Context> context)
        {
            if (stream.Current == '\'' || stream.Current == '"')
            {
                var start = stream.Position;
                var open = stream.Current;

                while (stream.Advance())
                {
                    var length = stream.Position - start;
                    if (ParseDollar(stream, semanticModel, context))
                    {
                        semanticModel.Tokens.Add(Classifications.String, start, length);
                        if (stream.Advance() == false || stream.Current == Constants.NewLine) return true;
                        start = stream.Position;
                    }

                    if (stream.Current == open)
                    {
                        if (stream.Peek(-1) != '\\')
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

        private bool ParseComment(Stream stream, SemanticModel semanticModel, Stack<Context> context)
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
                        if (ParseDollar(stream, semanticModel, context))
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

                        if (ParseDollar(stream, semanticModel, context))
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
            if (operators.Contains(stream.Current))
            {
                semanticModel.Tokens.Add(Classifications.Operator, stream.Position);
                return true;
            }

            return false;
        }

        private bool ParseKeywords(Stream stream, SemanticModel semanticModel)
        {
            var name = stream.PeekWord();

            if (name == null) return false;

            if (keywords.Contains(name))
            {
                semanticModel.Tokens.Add(Classifications.Keyword, stream.Position, name.Length);
            }

            stream.Advance(name.Length - 1);
            return true;
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
    }
}