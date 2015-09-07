using System;

namespace Typewriter.TemplateEditor.Lexing
{
    public class Token
    {
        public int Start { get; set; }
        public int Length { get; set; }
        public string Classification { get; set; }
        public string QuickInfo { get; set; }
        public Token MatchingToken { get; set; }
        public bool IsOpen { get; set; }
        public bool IsError { get; set; }
    }
}