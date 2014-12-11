using System;

namespace Typewriter.TemplateEditor.Lexing
{
    public class Token
    {
        public int Line { get; set; }

        public int Start { get; set; }
        public int Length { get; set; }
        public string Classification { get; set; }
        public string QuickInfo { get; set; }
    }
}