using System;
using System.Text;

namespace Typewriter.TemplateEditor.Lexing
{
    internal class Stream
    {
        private readonly int offset;
        private readonly string template;
        private int position = -1;
        private char current = char.MinValue;

        public Stream(string template, int offset = 0)
        {
            this.offset = offset;
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

        public string PeekLine(int start = 0)
        {
            var line = new StringBuilder();
            var i = start;
            do
            {
                line.Append(Peek(i));
                i++;
            } while (Peek(i) != '\n' && i + position < template.Length);

            line.Append('\n');

            return line.ToString();
        }

        public string PeekBlock(int start, char open, char close)
        {
            var i = start;
            var depth = 1;
            var identifier = new StringBuilder();
            
            while (depth > 0)
            {
                var letter = Peek(i);

                if (letter == char.MinValue) break;
                //if (letter == close) depth--;
                if (IsMatch(i, letter, close)) depth--;
                if (depth > 0)
                {
                    identifier.Append(letter);
                    //if (letter == open) depth++;
                    if (IsMatch(i, letter, open)) depth++;

                    i++;

                    if (letter != open && (letter == '"' || letter == '\''))
                    {
                        var block = PeekBlock(i, letter, letter);
                        identifier.Append(block);
                        i += block.Length;

                        if (letter == Peek(i))
                        {
                            identifier.Append(letter);
                            i++;
                        }
                    }
                }
            }

            return identifier.ToString();
        }

        private bool IsMatch(int index, char letter, char match)
        {
            if (letter == match)
            {
                var isString = match == '"' || match == '\'';
                if (isString)
                {
                    if (Peek(index - 1) == '\\' && Peek(index - 2) != '\\') return false;
                }

                return true;
            }

            return false;
        }

        public bool SkipWhitespace()
        {
            if (position < 0) Advance();

            while (char.IsWhiteSpace(Current))
            {
                Advance();
            }

            return position < template.Length;
        }
    }
}