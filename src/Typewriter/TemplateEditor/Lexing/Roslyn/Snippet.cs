using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typewriter.TemplateEditor.Lexing.Roslyn
{
    public class Snippet
    {
        public static Snippet Create(SnippetType type, string code, int offset = 0, int startIndex = -1, int endIndex = -1, int internalOffset = 0)
        {
            return new Snippet
            {
                Type = type,
                Code = code,
                Length = code.Length,
                StartIndex = startIndex,
                EndIndex = endIndex,
                Offset = offset,
                InternalOffset = internalOffset
            };
        }

        public SnippetType Type { get; private set; }
        public string Code { get; private set; }
        public int Length { get; set; }
        public int Offset { get; private set; }
        private int StartIndex { get; set; }
        private int EndIndex { get; set; }
        private int InternalOffset { get; set; }

        public int FromShadowIndex(int index)
        {
            return index + StartIndex - Offset + InternalOffset;
        }

        public int ToShadowIndex(int index)
        {
            return index - StartIndex + Offset - InternalOffset;
        }

        public bool Contains(int index)
        {
            return StartIndex <= index - InternalOffset && EndIndex >= index - InternalOffset;
        }
    }

    public enum SnippetType
    {
        Using,
        Code,
        Lambda,
        Class
    }
}
