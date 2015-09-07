using System.Collections.Generic;
using System.Linq;

namespace Typewriter.TemplateEditor.Lexing
{
    public class ContextSpans
    {
        private readonly List<ContextSpan> contexts = new List<ContextSpan>();

        public void Add(Context context, Context parentContext, ContextType type, int start, int end)
        {
            contexts.Add(new ContextSpan(start, end, context, parentContext, type));
        }

        public ContextSpan GetContextSpan(int position)
        {
            return contexts.Where(c => c.Start <= position && c.End >= position).OrderByDescending(c => c.Start).FirstOrDefault();
        }

        public IEnumerable<ContextSpan> GetContextSpans(ContextType type)
        {
            return contexts.Where(c => c.Type == type);
        }
    }
}