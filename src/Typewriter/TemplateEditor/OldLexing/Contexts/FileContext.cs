using System;
using Typewriter.TemplateEditor.Lexing.Identifiers;

namespace Typewriter.TemplateEditor.Lexing.Contexts
{
    public class FileContext : ContextBase
    {
        public FileContext()
        {
            identifiers.Add("Classes", new Identifier
            {
                Name = "Classes",
                Type = IdentifierType.Indexed,
                QuickInfo = "Loop over all classes in the file",
                Context = new ClassContext()
            });
            identifiers.Add("Enums", new Identifier(IdentifierType.Indexed));
            identifiers.Add("FullName", new Identifier(IdentifierType.Simple));
            identifiers.Add("fullName", new Identifier(IdentifierType.Simple));
            identifiers.Add("Interfaces", new Identifier(IdentifierType.Indexed));
            identifiers.Add("Name", new Identifier(IdentifierType.Simple));
            identifiers.Add("name", new Identifier(IdentifierType.Simple));
            identifiers.Add("Structs", new Identifier(IdentifierType.Indexed));
        }
    }
}