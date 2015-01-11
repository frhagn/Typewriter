using System;

namespace Typewriter.TemplateEditor.Lexing
{
    public class Identifier
    {
        public string Name { get; set; }
        public string QuickInfo { get; set; }
        public string Context { get; set; }
        public bool IsCollection { get; set; }
        public bool IsBoolean { get; set; }
        public bool HasContext { get; set; }
    }
}