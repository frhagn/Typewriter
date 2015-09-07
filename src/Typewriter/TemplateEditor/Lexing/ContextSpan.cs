using System;

namespace Typewriter.TemplateEditor.Lexing
{
    public class ContextSpan
    {
        public ContextSpan(int start, int end, Context context, Context parentContext, ContextType type)
        {
            Start = start;
            End = end;
            Context = context;
            ParentContext = parentContext;
            Type = type;
        }

        public int Start { get; private set; }
        public int End { get; private set; }
        public Context Context { get; private set; }
        public Context ParentContext { get; private set; }
        public ContextType Type { get; }
    }
}