using System;

namespace Typewriter.TemplateEditor.Lexing.Scopes
{
    public enum Scope
    {
        None,
        Statement,
        Block,
        Filter,
        Template,
        Separator,
        True,
        False
    }
}