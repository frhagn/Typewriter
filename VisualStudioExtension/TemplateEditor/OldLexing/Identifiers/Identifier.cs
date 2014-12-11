using System;
using Typewriter.TemplateEditor.Lexing.Contexts;

namespace Typewriter.TemplateEditor.Lexing.Identifiers
{
    public class Identifier
    {
        public Identifier()
        {   
        }

        public Identifier(IdentifierType type)
        {
            Type = type;
        }

        public string Name { get; set; }
        public IdentifierType Type { get; set; }
        public string QuickInfo { get; set; }
        public IContext Context { get; set; }
    }
}