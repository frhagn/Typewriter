using System;
using Typewriter.TemplateEditor.Lexing;

namespace Typewriter.Generation
{
    public static class TemplateParser
    {
        private static int counter;
        private const string methodTemplate = "public static object {0}({1} {2}){{ try {{ {3} }} catch(Exception __exception) {{ Typewriter.VisualStudio.Log.Error(__exception.Message); throw; }} }}";
        private const string classTemplate = @"
            namespace Typewriter
            {{
                using System;
                using System.Linq;
                using Typewriter.CodeModel;
                using Typewriter.Generation;

                public class Code
                {{
                    {0}
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
                if (ParseCodeBlock(stream, ref code)) continue;
                if (ParseLambda(stream, ref code, ref output)) continue;
                output += stream.Current;
            }

            if (code != string.Empty)
            {
                extensions = Compiler.Compile(string.Format(classTemplate, code));
            }

            return output;
        }

        private static bool ParseCodeBlock(Stream stream, ref string output)
        {
            if (stream.Current != '$' || stream.Peek() != '{') return false;

            var code = new Stream(ParseBlock(stream, '{', '}'));

            while (code.Advance())
            {
                if (code.PeekWord() == "var")
                {
                    code.Advance(3);
                    ParseWhitespace(code);

                    var name = code.PeekWord(1);
                    if (name == null) continue;

                    code.Advance(name.Length);
                    ParseWhitespace(code);

                    if (code.Peek() != '=') continue;
                    code.Advance();

                    ParseWhitespace(code);

                    var parameter = ParseBlock(code, '(', ')');
                    if (parameter == null) continue;

                    parameter = parameter.Trim(' ', '\t');
                    var index = parameter.IndexOf(' ');
                    if (index < 0) index = parameter.IndexOf('\t');
                    if (index < 0) continue;

                    var type = parameter.Substring(0, index);
                    parameter = parameter.Substring(index);

                    ParseWhitespace(code);

                    if (code.Peek() != '=' && code.Peek(2) != '>') continue;
                    code.Advance(2);

                    ParseWhitespace(code);

                    var body = ParseBlock(code, '{', '}');
                    if (body == null)
                    {
                        body = "return ";

                        do
                        {
                            body += code.Current;
                            if (code.Current == ';') break;
                        }
                        while (code.Advance());
                    }

                    try
                    {
                        type = Contexts.Find(type).Type.FullName;
                    }
                    catch
                    {
                        continue;
                    }

                    output += string.Format(methodTemplate, name, type, parameter, body);
                }
            }

            return true;
        }

        private static bool ParseLambda(Stream stream, ref string output, ref string template)
        {
            if (stream.Current == '$')
            {
                var identifier = stream.PeekWord(1);
                if (identifier != null)
                {
                    var filter = stream.PeekBlock(identifier.Length + 2, '(', ')');
                    if (filter != null && stream.Peek(filter.Length + 2 + identifier.Length + 1) == '[')
                    {
                        if (filter.Contains("=>"))
                        {
                            var name = "lambda" + counter++;

                            var code = new Stream(" " + filter);

                            while (code.Advance())
                            {
                                ParseWhitespace(code);

                                var parameter = ParseBlock(code, '(', ')');
                                if (parameter == null)
                                {
                                    parameter = code.PeekWord(1);
                                    if (parameter == null) return false;
                                    code.Advance(parameter.Length);
                                }
                                ParseWhitespace(code);

                                if (code.Peek() != '=' && code.Peek(2) != '>') return false;
                                code.Advance(2);

                                ParseWhitespace(code);

                                var body = ParseBlock(code, '{', '}');
                                if (body == null)
                                {
                                    body = "return ";

                                    do
                                    {
                                        body += code.Current;
                                    }
                                    while (code.Advance());

                                    body += ";";
                                }

                                try
                                {
                                    var type = Contexts.Find(identifier).Type.FullName;

                                    output += string.Format(methodTemplate, name, type, parameter, body);
                                    stream.Advance(filter.Length + 2 + identifier.Length);
                                    template += $"${identifier}(${name})";
                                }
                                catch
                                {
                                    return false;
                                }

                                return true;
                            }
                        }
                    }
                }
            }

            return false;
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