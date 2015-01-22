using System;
using System.Collections.Generic;
using System.Linq;
using Typewriter.TemplateEditor.Lexing;
using Typewriter.VisualStudio;

namespace Typewriter.Generation
{
    public class Parser
    {
        private static readonly Type standardExtensions = typeof(Extensions);

        public static string Parse(string template, Type customExtensions, object context)
        {
            var instance = new Parser(customExtensions);
            var output = instance.ParseTemplate(template, context);

            return instance.matchFound ? output : null;
        }

        private readonly Type customExtensions;
        private bool matchFound;

        private Parser(Type customExtensions)
        {
            this.customExtensions = customExtensions;
        }

        private string ParseTemplate(string template, object context)
        {
            if (string.IsNullOrWhiteSpace(template)) return null;

            var output = string.Empty;
            var stream = new Stream(template);

            while (stream.Advance())
            {
                if (ParseDollar(stream, context, ref output)) continue;
                output += stream.Current;
            }

            return output;
        }

        private bool ParseDollar(Stream stream, object context, ref string output)
        {
            if (stream.Current == '$')
            {
                var identifier = stream.PeekWord(1);
                var value = GetIdentifier(identifier, context);

                if (value != null)
                {
                    stream.Advance(identifier.Length);

                    var collection = value as IEnumerable<object>;
                    if (collection != null)
                    {
                        var filter = ParseBlock(stream, '(', ')');
                        var block = ParseBlock(stream, '[', ']');
                        var separator = ParseBlock(stream, '[', ']');

                        var items = ItemFilter.Apply(collection, filter, ref matchFound);
                        output += string.Join(ParseTemplate(separator, context), items.Select(item => ParseTemplate(block, item)));
                    }
                    else if (value is bool)
                    {
                        var trueBlock = ParseBlock(stream, '[', ']');
                        var falseBlock = ParseBlock(stream, '[', ']');

                        output += ParseTemplate((bool)value ? trueBlock : falseBlock, context);
                    }
                    else
                    {
                        var block = ParseBlock(stream, '[', ']');
                        if (block != null)
                        {
                            output += ParseTemplate(block, value);
                        }
                        else
                        {
                            output += value.ToString();
                        }
                    }

                    return true;
                }
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

        private object GetIdentifier(string identifier, object context)
        {
            if (identifier == null) return null;

            var type = context.GetType();

            try
            {
                if (customExtensions != null)
                {
                    var c = customExtensions.GetMethod(identifier, new[] { type });
                    if (c != null)
                    {
                        return c.Invoke(null, new[] { context });
                    }
                }

                var extension = standardExtensions.GetMethod(identifier, new[] { type });
                if (extension != null)
                {
                    return extension.Invoke(null, new[] { context });
                }

                var property = type.GetProperty(identifier);
                if (property != null)
                {
                    return property.GetValue(context);
                }
            }
            catch(Exception e)
            {
                Log.Error(e.Message);
            }

            return null;
        }
    }
}
