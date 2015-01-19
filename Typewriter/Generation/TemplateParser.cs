using System;
using System.Collections.Generic;
using System.Linq;
using Typewriter.TemplateEditor.Lexing;

namespace Typewriter.Generation
{
    public static class TemplateParser
    {
        public static string Parse(string template, ref Type extensions)
        {
            if (string.IsNullOrWhiteSpace(template)) return null;

            extensions = Compiler.Compile(@"
namespace Test
{
    using System;
    using System.Linq;
    using Typewriter.CodeModel;

    public class Code" + DateTime.Now.Ticks + @"
    {
        public static object Test(dynamic item)
        {
            return item.FullName;
        }
    }
}
");

            var output = string.Empty;
            var stream = new Stream(template);

            while (stream.Advance())
            {
                if (ParseDollar(stream, ref output)) continue;
                output += stream.Current;
            }

            return output;
        }

        private static bool ParseDollar(Stream stream, ref string output)
        {
            if (stream.Current == '$' && stream.Peek() == '{')
            {
                var code = ParseBlock(stream, '{', '}');

                return true;
            }

            return false;
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

        //private object GetIdentifier(string identifier, object context)
        //{
        //    if (identifier == null) return null;

        //    var type = context.GetType();

        //    try
        //    {
        //        var c = code.GetMethod(identifier, new[] { type });
        //        if (c != null)
        //        {
        //            return c.Invoke(null, new[] { context });
        //        }

        //        var extension = extensions.GetMethod(identifier, new[] { type });
        //        if (extension != null)
        //        {
        //            return extension.Invoke(null, new[] { context });
        //        }

        //        var property = type.GetProperty(identifier);
        //        if (property != null)
        //        {
        //            return property.GetValue(context);
        //        }
        //    }
        //    catch
        //    {
        //    }

        //    return null;
        //}
    }
}
