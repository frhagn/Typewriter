using System;
using Typewriter.TemplateEditor.Lexing.Identifiers;

namespace Typewriter.TemplateEditor.Lexing.Contexts
{
    public class EnumContext : ContextBase
    {
        public EnumContext()
        {
            identifiers.Add("FullName", new Identifier(IdentifierType.Simple));
            identifiers.Add("fullName", new Identifier(IdentifierType.Simple));
            identifiers.Add("Name", new Identifier(IdentifierType.Simple));
            identifiers.Add("name", new Identifier(IdentifierType.Simple));
            identifiers.Add("Value", new Identifier(IdentifierType.Simple));
            identifiers.Add("value", new Identifier(IdentifierType.Simple));
        }
    }
}