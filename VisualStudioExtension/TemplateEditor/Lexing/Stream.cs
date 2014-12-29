using System;
using System.Text;

namespace Typewriter.TemplateEditor.Lexing
{
    internal class Stream
    {
        private readonly int offset;
        private readonly int lineOffset;
        private readonly string template;
        private int position = -1;
        private int line;
        private char current = char.MinValue;

        public Stream(string template, int offset = 0, int lineOffset = 0)
        {
            this.offset = offset;
            this.lineOffset = lineOffset;
            this.template = template ?? string.Empty;
        }

        public int Position
        {
            get { return position + offset; }
        }

        public char Current
        {
            get { return current; }
        }

        public int Line {
            get { return line + lineOffset; }
        }

        public bool Advance(int offset = 1)
        {
            for (int i = 0; i < offset; i++)
            {
                position ++;

                if (position >= template.Length)
                {
                    current = char.MinValue;
                    return false;
                }

                current = template[position];
                if (current == Constants.NewLine) line++;
            }

            return true;
        }

        public char Peek(int offset = 1)
        {
            var index = position + offset;

            if (index > -1 && index < template.Length)
            {
                return template[index];
            }

            return char.MinValue;
        }

        public string PeekWord(int start = 0)
        {
            if (char.IsLetter(Peek(start)) == false) return null;

            var identifier = new StringBuilder();
            var i = start;
            while (char.IsLetterOrDigit(Peek(i)))
            {
                identifier.Append(Peek(i));
                i++;
            }

            return identifier.ToString();
        }

        public string PeekBlock(int start = 0, char open = '[', char close = ']')
        {
            var i = start;
            var depth = 1;
            var identifier = new StringBuilder();
            
            while (depth > 0)
            {
                var letter = Peek(i);

                if (letter == char.MinValue) break;
                if (letter == open) depth++;
                if (letter == close) depth--;
                if (depth > 0) identifier.Append(letter);

                i++;
            }

            return identifier.ToString();
        }
    }
}