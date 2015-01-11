using System;
using Typewriter.TemplateEditor.Lexing.Identifiers;

namespace Typewriter.TemplateEditor.Lexing.Contexts
{
    public class ClassContext : ContextBase
    {
        public ClassContext()
        {
            identifiers.Add("FullName", new Identifier
            {
                Name = "FullName",
                Type = IdentifierType.Simple,
                QuickInfo = "Returns the full name (name and namespace) of the class"
            });

            identifiers.Add("Name", new Identifier
            {
                Name = "Name",
                Type = IdentifierType.Simple,
                QuickInfo = "Returns the name of the class"
            });

            identifiers.Add("name", new Identifier
            {
                Name = "name",
                Type = IdentifierType.Simple,
                QuickInfo = "Returns a camelCase version of the name of the class"
            });

            identifiers.Add("Namespace", new Identifier
            {
                Name = "Namespace",
                Type = IdentifierType.Simple,
                QuickInfo = "Returns the namespace of the class"
            });
        }
    }
}