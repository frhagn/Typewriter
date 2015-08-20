using System;
using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel;
using Typewriter.TemplateEditor.Lexing;
using Typewriter.VisualStudio;
using Type = System.Type;

namespace Typewriter.Generation
{
    public class Parser
    {
        public static string Parse(string template, List<Type> extensions, object context, out bool success)
        {
            var instance = new Parser(extensions);
            var output = instance.ParseTemplate(template, context);
            success = instance.hasError == false;

            return instance.matchFound ? output : null;
        }

        private readonly List<Type> extensions;
        private bool matchFound;
        private bool hasError;

        private Parser(List<Type> extensions)
        {
            this.extensions = extensions;
        }

        private string ParseTemplate(string template, object context)
        {
            if (string.IsNullOrEmpty(template)) return null;

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
                object value;
                
                if (TryGetIdentifier(identifier, context, out value))
                {
                    stream.Advance(identifier.Length);

                    var collection = value as IEnumerable<Item>;
                    if (collection != null)
                    {
                        var filter = ParseBlock(stream, '(', ')');
                        var block = ParseBlock(stream, '[', ']');
                        var separator = ParseBlock(stream, '[', ']');

                        if (filter == null && block == null && separator == null)
                        {
                            var stringValue = value.ToString();

                            if (stringValue != value.GetType().FullName)
                                output += stringValue;
                            else
                                output += "$" + identifier;
                        }
                        else
                        {
                            IEnumerable<Item> items;
                            if (filter != null && filter.StartsWith("$"))
                            {
                                var predicate = filter.Remove(0, 1);
                                if (extensions != null)
                                {
                                    // Lambda filters are always defined in the first extension type
                                    var c = extensions.FirstOrDefault()?.GetMethod(predicate);
                                    if (c != null)
                                    {
                                        items = collection.Where(x => (bool)c.Invoke(null, new object[] { x })).ToList();
                                        matchFound = matchFound || items.Any();
                                    }
                                    else
                                    {
                                        items = new Item[0];
                                    }
                                }
                                else
                                {
                                    items = new Item[0];
                                }
                            }
                            else
                            {
                                items = ItemFilter.Apply(collection, filter, ref matchFound);
                            }
                            output += string.Join(ParseTemplate(separator, context), items.Select(item => ParseTemplate(block, item)));
                        }
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
                        if (value != null)
                        {
                            if (block != null)
                            {
                                output += ParseTemplate(block, value);
                            }
                            else
                            {
                                output += value.ToString();
                            }
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

        private bool TryGetIdentifier(string identifier, object context, out object value)
        {
            value = null;

            if (identifier == null) return false;

            var type = context.GetType();

            try
            {
                var property = type.GetProperty(identifier);
                if (property != null)
                {
                    value = property.GetValue(context);
                    return true;
                }

                var extension = extensions.Select(e => e.GetMethod(identifier, new[] { type })).FirstOrDefault(m => m != null);
                if (extension != null)
                {
                    value = extension.Invoke(null, new[] { context });
                    return true;
                }
            }
            catch (Exception e)
            {
                hasError = true;
                Log.Error(e.Message);
            }

            return false;
        }
    }
}
