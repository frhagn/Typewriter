using System;
using System.Collections.Generic;
using System.Text;
using Typewriter.CodeModel;

namespace Typewriter.TemplateEditor.Lexing
{
    public class CodeLexer
    {
        private readonly Contexts contexts;
        private readonly Context fileContext;
        
        private SemanticModel semanticModel;
        private Stack<Context> context;

        public CodeLexer(Contexts contexts)
        {
            this.contexts = contexts;
            this.fileContext = contexts.Find(nameof(File));
        }

        public void Tokenize(SemanticModel semanticModel, string code)
        {
            this.semanticModel = semanticModel;
            context = new Stack<Context>(new[] { fileContext });

            semanticModel.ShadowClass.Clear();

            Parse(code, 0);

            semanticModel.ShadowClass.Parse();
            
            semanticModel.Tokens.AddRange(semanticModel.ShadowClass.GetTokens());
            semanticModel.ErrorTokens.AddRange(semanticModel.ShadowClass.GetErrorTokens());
            semanticModel.TempIdentifiers.Add(semanticModel.ShadowClass.GetIdentifiers(contexts));
        }

        private void Parse(string template, int offset)
        {
            var stream = new Stream(template, offset);

            do
            {
                ParseCodeBlock(stream);
                ParseDollar(stream);
            }
            while (stream.Advance());
        }

        private void ParseCodeBlock(Stream stream)
        {
            if (stream.Current == '$' && stream.Peek() == '{' && context.Peek() == fileContext)
            {
                stream.Advance();

                var block = stream.PeekBlock(1, '{', '}');

                semanticModel.ContextSpans.Add(null, null, ContextType.CodeBlock, stream.Position + 1, stream.Position + block.Length + 1);
                
                var codeStream = new Stream(block, stream.Position + 1);

                ParseUsings(codeStream);
                ParseCode(codeStream);

                stream.Advance(block.Length);
            }
        }

        private void ParseUsings(Stream stream)
        {
            stream.Advance();
            
            while (true)
            {
                stream.SkipWhitespace();

                if ((stream.Current == 'u' && stream.PeekWord() == "using") || (stream.Current == '/' && stream.Peek() == '/'))
                {
                    var line = stream.PeekLine();
                    semanticModel.ShadowClass.AddUsing(line, stream.Position);
                    stream.Advance(line.Length);

                    continue;
                }

                break;
            }
        }

        private void ParseCode(Stream stream)
        {
            var code = new StringBuilder();
            var position = stream.Position;
            var isString = false;
            var open = char.MinValue;

            do
            {
                if (stream.Current != char.MinValue)
                    code.Append(stream.Current);

                if (isString && stream.Current == open && stream.Peek(-1) != '\\')
                {
                    isString = false;
                }
                else if (stream.Current == '"' || stream.Current == '\'')
                {
                    open = stream.Current;
                    isString = true;
                }
                
                if(isString == false)
                {
                    semanticModel.Tokens.AddBrace(stream);
                }
            }
            while (stream.Advance());

            semanticModel.ShadowClass.AddBlock(code.ToString(), position);
        }

        private void ParseDollar(Stream stream)
        {
            if (stream.Current == '$')
            {
                var identifier = GetIdentifier(stream);
                if (identifier != null)
                {
                    stream.Advance(identifier.Name.Length);

                    if (identifier.IsCollection)
                    {
                        context.Push(contexts.Find(identifier.Context));
                        ParseFilter(stream);
                        ParseBlock(stream); // template

                        context.Pop();

                        ParseBlock(stream); // separator
                    }
                    else if (identifier.IsBoolean)
                    {
                        ParseBlock(stream); // true
                        ParseBlock(stream); // false
                    }
                    else if (identifier.IsParent)
                    {
                        var current = context.Pop();
                        
                        ParseBlock(stream); // template

                        context.Push(current);
                    }
                    else if (identifier.HasContext)
                    {
                        context.Push(contexts.Find(identifier.Context));

                        ParseBlock(stream); // template

                        context.Pop();
                    }
                }
            }
        }

        private void ParseFilter(Stream stream)
        {
            if (stream.Peek() == '(')
            {
                stream.Advance();

                var block = stream.PeekBlock(1, '(', ')');
                var index = block.IndexOf("=>", StringComparison.Ordinal);

                if (index > 0)
                {
                    var name = block.Substring(0, index);

                    semanticModel.ContextSpans.Add(null, null, ContextType.Lambda, stream.Position + 1, stream.Position + block.Length + 1);
                    semanticModel.ShadowClass.AddLambda(block, context.Peek().Name, name, stream.Position + 1);
                }
                stream.Advance(block.Length);

                if (stream.Peek() == ')')
                {
                    stream.Advance();
                }
            }
        }

        private void ParseBlock(Stream stream)
        {
            if (stream.Peek() == '[')
            {
                stream.Advance();

                var block = stream.PeekBlock(1, '[', ']');
                Parse(block, stream.Position + 1);
                stream.Advance(block.Length);
            }
        }

        private Identifier GetIdentifier(Stream stream)
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