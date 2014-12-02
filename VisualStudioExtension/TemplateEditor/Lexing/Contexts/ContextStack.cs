using System;
using System.Collections.Generic;

namespace Typewriter.TemplateEditor.Lexing.Contexts
{
    public class ContextStack : Stack<IContext>
    {
        private static readonly Dictionary<string, IContext> contexts = new Dictionary<string, IContext>
        {
            { "File", new FileContext() },
            { "Classes", new ClassContext() },
            { "Enums", new EnumContext() }
        };

        public ContextStack()
        {
            Push("File");
        }

        public IContext Current
        {
            get { return Peek(); }
        }

        public IContext Push(string name)
        {
            IContext context;
            if (name != null && contexts.TryGetValue(name, out context))
            {
                Push(context);
                return context;
            }

            return null;
        }
    }
}