using System;
using System.Collections.Generic;
using System.Linq;

namespace Typewriter.TemplateEditor.Lexing
{
    public class Context 
    {
        private readonly Dictionary<string, Identifier> identifiers = new Dictionary<string, Identifier>();
        private readonly Dictionary<string, Identifier> extensionIdentifiers = new Dictionary<string, Identifier>();

        public Context(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; private set; }
        public Type Type { get; private set; }
        public ICollection<Identifier> Identifiers => identifiers.Values;

        public void AddIdentifier(Identifier identifier)
        {
            identifiers.Add(identifier.Name, identifier);
        }

        public void AddExtensionIdentifier(string extensionNamespace, Identifier identifier)
        {
            extensionIdentifiers.Add(extensionNamespace + "." + identifier.Name, identifier);
        }

        public Identifier GetIdentifier(string name)
        {
            if (name == null) return null;

            Identifier i;
            return identifiers.TryGetValue(name, out i) ? i : null;
        }

        public Identifier GetExtensionIdentifier(string extensionNamespace, string name)
        {
            if (name == null) return null;

            Identifier i;
            return extensionIdentifiers.TryGetValue(extensionNamespace + "." + name, out i) ? i : null;
        }

        public IEnumerable<Identifier> GetExtensionIdentifiers(string extensionNamespace)
        {
            return extensionIdentifiers.Where(i => i.Key.StartsWith(extensionNamespace + ".")).Select(i => i.Value);
        }
    }

    public enum ContextType
    {
        Template,
        CodeBlock,
        Lambda
    }
}