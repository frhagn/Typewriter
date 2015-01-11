using System;
using System.Collections.Generic;
using Typewriter.TemplateEditor.Lexing.Identifiers;

namespace Typewriter.TemplateEditor.Lexing.Contexts
{
    public class ContextBase : IContext
    {
        protected readonly Dictionary<string, Identifier> identifiers = new Dictionary<string, Identifier>();

        public Identifier GetIdentifier(string name)
        {
            Identifier i;
            return name != null && identifiers.TryGetValue(name, out i) ? i : null;
        }

        public ICollection<Identifier> Identifiers
        {
            get { return identifiers.Values; }
        }
    }
}