using System;
using System.Linq;
using Typewriter.TemplateEditor.Lexing.Braces;
using Typewriter.TemplateEditor.Lexing.Contexts;
using Typewriter.TemplateEditor.Lexing.Identifiers;
using Typewriter.TemplateEditor.Lexing.Scopes;
using Typewriter.TemplateEditor.Lexing.Tokens;

namespace Typewriter.TemplateEditor.Lexing
{
    public class Lexer
    {

        private static readonly char[] whitespaceChars = { ' ', '\t', '\r', '\n', '\f' };
        private readonly string[] keywords = { "class", "module", "private", "public", "export", "if", "else" };

        public TokenList Tokenize(string template)
        {
            //var x = ClassificationRegistry.GetClassificationType(Constants.KeywordClassificationType);


            var stream = new TemplateStream(template);

            var tokens = new TokenList();
            var context = new ContextStack(tokens);
            var scope = new ScopeStack();
            var brace = new BraceStack();

            while (true)
            {
                if (ParseComment(stream, tokens, context))
                    continue;

                if (ParseDollar(stream, tokens, context, scope))
                    continue;

                if (ParseStatement(stream, tokens, context, scope))
                    continue;

                if (ParseFilter(stream, tokens, context, scope))
                    continue;

                if (ParseOther(stream, tokens, context, scope, brace))
                    continue;

                if (stream.Advance() == false)
                    break;
            }

            context.Clear(stream.Position);

            return tokens;
        }

        private bool ParseComment(TemplateStream stream, TokenList tokens, ContextStack context)
        {
            if (stream.Current != '/' || stream.Peek() != '/') return false;

            var start = stream.Position;
            var line = stream.Line;
            while (stream.Advance())
            {
                if (stream.Peek() == '\r')
                {
                    stream.Advance();
                    break;
                }
            }

            tokens.Add(new Token(start, stream.Position - start, line, TokenType.Comment, context.Current));

            return true;
        }

        private bool ParseDollar(TemplateStream stream, TokenList tokens, ContextStack context, ScopeStack scope)
        {
            if (stream.Current != '$' || scope.Current == Scope.Block) return false;

            var next = stream.Peek();
            if (next == '$')
            {
                stream.Advance(2);
                return true;
            }

            if (char.IsLetter(next))
            {
                scope.Push(Scope.Statement);
                stream.Advance();
                return true;
            }

            if (next == '{')
            {
                scope.Push(Scope.Block);
                stream.Advance();
                return true;
            }

            return false;
        }

        private bool ParseStatement(TemplateStream stream, TokenList tokens, ContextStack context, ScopeStack scope)
        {
            if (scope.Current == Scope.Statement)
            {
                scope.Pop();
            }
            else if (scope.Current == Scope.Block)
            {
                var previous = stream.Peek(-1);
                if (previous == '$' || char.IsLetterOrDigit(previous)) return false;
            }
            else
            {
                return false;
            }

            var name = stream.PeekWord();
            var identifier = context.Current.GetIdentifier(name);

            if (identifier != null)
            {
                tokens.Add(new Token(stream.Position, name.Length, stream.Line, TokenType.Identifier, context.Current, identifier.QuickInfo));
                stream.Advance(name.Length);

                if (identifier.Type == IdentifierType.Indexed)
                {
                    if (stream.Current == '(') scope.Push(Scope.Filter);
                    if (stream.Current == '[')
                    {
                        scope.Push(Scope.Template);
                        context.Push(name, stream.Position);
                    }
                }
                else if (identifier.Type == IdentifierType.Boolean)
                {
                    if (stream.Current == '[') scope.Push(Scope.True);
                }

                return true;
            }

            return false;
        }

        private bool ParseFilter(TemplateStream stream, TokenList tokens, ContextStack context, ScopeStack scope)
        {
            //if (scope.Current != Scope.Filter) return false;

            //scope.Pop();

            //tokens.Add(new Token(stream.Position, 1, TokenType.OpenFunctionBrace, context.Current));

            //while (stream.Current != ')')
            //    if (stream.Advance() == false) return true;

            //tokens.Add(new Token(stream.Position, 1, TokenType.CloseFunctionBrace, context.Current));

            //stream.Advance();

            //if (stream.Current == '[') scope.Push(Scope.Template);

            //return true;
            return false;
        }

        private bool ParseOther(TemplateStream stream, TokenList tokens, ContextStack context, ScopeStack scope, BraceStack brace)
        {
            switch (stream.Current)
            {
                case '[':
                    brace.Push(tokens.Add(new Token(stream.Position, 1, stream.Line, TokenType.OpenBrace, context.Current)), scope.Changed);
                    stream.Advance();
                    return true;

                case ']':
                    var openBrace = brace.Pop(TokenType.OpenBrace);
                    var token = tokens.Add(new Token(stream.Position, 1, stream.Line, TokenType.CloseBrace, context.Current, null, openBrace.Token));
                    if (openBrace.Token != null)
                    {
                        openBrace.Token.MatchingToken = token;
                    }
                    stream.Advance();

                    if (openBrace.ScopeChanged)
                    {
                        var current = scope.Pop();
                        if (current == Scope.Template)
                        {
                            context.Pop(stream.Position);
                            if (stream.Current == '[') scope.Push(Scope.Separator);
                        }
                        else if (current == Scope.True)
                        {
                            context.Pop(stream.Position);
                            if (stream.Current == '[') scope.Push(Scope.False);
                        }
                    }
                    return true;

                case '{':
                    //tokens.Add(new Token(stream.Position - 1, 2, TokenType.OpenBlock, context.Current));
                    brace.Push(tokens.Add(new Token(stream.Position, 1, stream.Line, TokenType.OpenCurlyBrace, context.Current)), scope.Changed);
                    stream.Advance();
                    return true;

                case '}':
                    var openCurlyBrace = brace.Pop(TokenType.OpenCurlyBrace);
                    token = tokens.Add(new Token(stream.Position, 1, stream.Line, TokenType.CloseCurlyBrace, context.Current, null, openCurlyBrace.Token));
                    if (openCurlyBrace.Token != null)
                    {
                        openCurlyBrace.Token.MatchingToken = token;
                    }
                    //tokens.Add(new Token(stream.Position, 1, TokenType.CloseBlock, context.Current));
                    stream.Advance();

                    if (openCurlyBrace.ScopeChanged)
                    {
                        scope.Pop();
                    }
                    return true;

                case '(':
                    brace.Push(tokens.Add(new Token(stream.Position, 1, stream.Line, TokenType.OpenFunctionBrace, context.Current)), scope.Changed);
                    stream.Advance();
                    return true;

                case ')':
                    var openFunctionBrace = brace.Pop(TokenType.OpenFunctionBrace);
                    token = tokens.Add(new Token(stream.Position, 1, stream.Line, TokenType.CloseFunctionBrace, context.Current, null, openFunctionBrace.Token));
                    if (openFunctionBrace.Token != null)
                    {
                        openFunctionBrace.Token.MatchingToken = token;
                    }
                    stream.Advance();

                    if (openFunctionBrace.ScopeChanged)
                    {
                        scope.Pop();
                    }
                    return true;
            }

            if (scope.Current == Scope.Block) return false;

            var name = stream.PeekWord();

            if (name == null) return false;

            if (keywords.Contains(name))
            {
                tokens.Add(new Token(stream.Position, name.Length, stream.Line, TokenType.Keyword, context.Current));
            }

            stream.Advance(name.Length);
            return true;
        }
    }
}