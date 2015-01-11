using System;
using System.Collections.Generic;
using System.Drawing;
using Typewriter.TemplateEditor.Lexing.Tokens;

namespace Typewriter.TemplateEditor.Lexing.Contexts
{
    public class ContextSpan
    {
        public ContextSpan(int start, int end, IContext context)
        {
            Start = start;
            End = end;
            Context = context;
        }

        public int Start { get; set; }
        public int End { get; set; }
        public IContext Context { get; set; }
    }

    public class ContextStack
    {
        private readonly TokenList tokens;
        private readonly Stack<ContextSpan> stack;

        private static readonly Dictionary<string, IContext> contexts = new Dictionary<string, IContext>
        {
            { "File", new FileContext() },
            { "Classes", new ClassContext() },
            { "Enums", new EnumContext() }
        };

        public ContextStack(TokenList tokens)
        {
            this.tokens = tokens;
            this.stack = new Stack<ContextSpan>();

            Push("File", 0);
        }

        public IContext Current
        {
            get { return stack.Peek().Context; }
        }

        public IContext Push(string name, int start)
        {
            IContext context;
            if (name != null && contexts.TryGetValue(name, out context))
            {
                stack.Push(new ContextSpan(start, 0, context));
                return context;
            }

            return null;
        }

        public IContext Pop(int end)
        {
            var span = stack.Pop();
            span.End = end;
            tokens.AddContext(span);

            return span.Context;
        }

        public void Clear(int end)
        {
            while (stack.Count > 0)
                Pop(end);
        }
    }
}