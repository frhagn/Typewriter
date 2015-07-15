using System;
using System.Text;
using Typewriter.TemplateEditor.Lexing;
using Typewriter.TemplateEditor.Lexing.Roslyn;

namespace Typewriter.Generation
{
    public static class TemplateCodeParser
    {
        private static readonly ShadowClass shadowClass = new ShadowClass();
        private static int counter;

        public static string Parse(string template, ref Type extensions)
        {
            if (string.IsNullOrWhiteSpace(template)) return null;

            var output = string.Empty;
            var stream = new Stream(template);

            shadowClass.Clear();

            while (stream.Advance())
            {
                if (ParseCodeBlock(stream)) continue;
                if (ParseLambda(stream, ref output)) continue;
                output += stream.Current;
            }

            shadowClass.Parse();
            var source = shadowClass.GetClass();

            extensions = Compiler.Compile(source);

            return output;
        }
        
        private static bool ParseCodeBlock(Stream stream)
        {
            if (stream.Current == '$' && stream.Peek() == '{')
            {
                stream.Advance();

                var block = stream.PeekBlock(1, '{', '}');
                var codeStream = new Stream(block, stream.Position + 1);

                ParseUsings(codeStream);
                ParseCode(codeStream);

                stream.Advance(block.Length + 1);

                return true;
            }

            return false;
        }

        private static void ParseUsings(Stream stream)
        {
            stream.Advance();

            while (true)
            {
                stream.SkipWhitespace();

                if ((stream.Current == 'u' && stream.PeekWord() == "using") || (stream.Current == '/' && stream.Peek() == '/'))
                {
                    var line = stream.PeekLine();
                    shadowClass.AddUsing(line, stream.Position);
                    stream.Advance(line.Length);

                    continue;
                }

                break;
            }
        }

        private static void ParseCode(Stream stream)
        {
            var code = new StringBuilder();

            do
            {
                code.Append(stream.Current);
            }
            while (stream.Advance());

            shadowClass.AddBlock(code.ToString(), 0);
        }

        private static bool ParseLambda(Stream stream, ref string template)
        {
            if (stream.Current == '$')
            {
                var identifier = stream.PeekWord(1);
                if (identifier != null)
                {
                    var filter = stream.PeekBlock(identifier.Length + 2, '(', ')');
                    if (filter != null && stream.Peek(filter.Length + 2 + identifier.Length + 1) == '[')
                    {
                        try
                        {
                            var index = filter.IndexOf("=>", StringComparison.Ordinal);

                            if (index > 0)
                            {
                                var name = filter.Substring(0, index);
                                var type = Contexts.Find(identifier)?.Type.FullName;

                                if (type == null) return false;

                                var methodIndex = counter++;

                                shadowClass.AddLambda(filter, type, name, methodIndex);

                                stream.Advance(filter.Length + 2 + identifier.Length);
                                template += $"${identifier}($__{methodIndex})";

                                return true;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return false;
        }
    }
}
