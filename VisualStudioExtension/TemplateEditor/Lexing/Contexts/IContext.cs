using System;
using System.Collections.Generic;
using Typewriter.TemplateEditor.Lexing.Identifiers;

namespace Typewriter.TemplateEditor.Lexing.Contexts
{
    public interface IContext
    {
        Identifier GetIdentifier(string name);
        ICollection<Identifier> Identifiers { get; }
    }
}