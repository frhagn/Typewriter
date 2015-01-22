using System;
using Typewriter.TemplateEditor.Lexing;

namespace Typewriter.Generation
{
    public static class TemplateParser
    {
        private const string classTemplate = @"
            namespace Typewriter{0}
            {{
                using System;
                using System.Linq;
                using Typewriter.CodeModel;

                public class Code
                {{
                    {1}
                }}
            }}";

        public static string Parse(string template, ref Type extensions)
        {
            if (string.IsNullOrWhiteSpace(template)) return null;

            var code = string.Empty;
            var output = string.Empty;
            var stream = new Stream(template);

            while (stream.Advance())
            {
                if (ParseDollar(stream, ref code)) continue;
                output += stream.Current;
            }

            if (code != string.Empty)
            {
                extensions = Compiler.Compile(string.Format(classTemplate, DateTime.Now.Ticks, code));
            }

            return output;
        }

        private static bool ParseDollar(Stream stream, ref string output)
        {
            if (stream.Current != '$' || stream.Peek() != '{') return false;

            var code = new Stream(ParseBlock(stream, '{', '}'));

            while (code.Advance())
            {
                if (code.PeekWord() == "declare")
                {
                    code.Advance(7);
                    ParseWhitespace(code);

                    var name = code.PeekWord(1);
                    if (name == null) continue;

                    code.Advance(name.Length);
                    ParseWhitespace(code);

                    var parameter = ParseBlock(code, '(', ')');
                    if (parameter == null) continue;

                    ParseWhitespace(code);

                    var body = ParseBlock(code, '{', '}');
                    if (body == null) continue;

                    output += string.Format("public static object {0}({1}){{ {2} }}", name, parameter, body);
                }
            }

            return true;
        }

        private static void ParseWhitespace(Stream stream)
        {
            while (char.IsWhiteSpace(stream.Peek()))
            {
                stream.Advance();
            }
        }

        private static string ParseBlock(Stream stream, char open, char close)
        {
            if (stream.Peek() == open)
            {
                var block = stream.PeekBlock(2, open, close);

                stream.Advance(block.Length);
                stream.Advance(stream.Peek(2) == close ? 2 : 1);

                return block;
            }

            return null;
        }
    }
}