using System;
using System.Text;

namespace Typewriter.TemplateEditor.Lexing
{
    internal class TemplateStream
    {
        private readonly string template;
        private int position = -1;
        private char current = '\0';

        public TemplateStream(string template)
        {
            this.template = template ?? string.Empty;
        }

        public int Position
        {
            get { return position; }
        }

        public char Current
        {
            get { return current; }
        }

        public int Line { get; private set; }

        public bool Advance(int offset = 1)
        {
            position += offset;

            if (position < template.Length)
            {
                current = template[position];
                if (current == '\r') Line++;
                return true;
            }

            current = '\0';
            return false;
        }

        public char Peek(int offset = 1)
        {
            var index = position + offset;

            if (index > -1 && index < template.Length)
            {
                return template[index];
            }

            return '\0';
        }

        public string PeekWord()
        {
            if (char.IsLetter(current) == false) return null;

            var identifier = new StringBuilder();
            var i = 0;
            while (char.IsLetterOrDigit(Peek(i)))
            {
                identifier.Append(Peek(i));
                i++;
            }

            return identifier.ToString();
        }
    }
}