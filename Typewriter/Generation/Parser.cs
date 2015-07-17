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
        private static readonly Type standardExtensions = typeof(Extensions);

        public static string Parse(string template, Type customExtensions, object context, out bool success)
        {
            var instance = new Parser(customExtensions);
            var output = instance.ParseTemplate(template, context);
            success = instance.hasError == false;

            return instance.matchFound ? output : null;
        }

        private readonly Type customExtensions;
        private bool matchFound;
        private bool hasError;

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
                object value;
                
                if (TryGetIdentifier(identifier, context, out value))
                {
                    stream.Advance(identifier.Length);

                    var collection = value as IEnumerable<CodeItem>;
                    if (collection != null)
                    {
                        var filter = ParseBlock(stream, '(', ')');
                        var block = ParseBlock(stream, '[', ']');
                        var separator = ParseBlock(stream, '[', ']');

                        IEnumerable<CodeItem> items;
                        if (filter != null && filter.StartsWith("$"))
                        {
                            var predicate = filter.Remove(0, 1);
                            if (customExtensions != null)
                            {
                                var c = customExtensions.GetMethod(predicate);
                                if (c != null)
                                {
                                    items = collection.Where(x => (bool)c.Invoke(null, new object[] { x })).ToList();
                                    matchFound = matchFound || items.Any();
                                }
                                else
                                {
                                    items = new CodeItem[0];
                                }
                            }
                            else
                            {
                                items = new CodeItem[0];
                            }
                        }
                        else
                        {
                            items = ItemFilter.Apply(collection, filter, ref matchFound);
                        }
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
                        if (value != null)
                        {
                            if (block != null)
                            {
                                output += ParseTemplate(block, value);
                            }
                            else
                            {
                                var extension = standardExtensions.GetMethod(identifier, new[] { value.GetType() });
                                if (extension != null && extension.ReturnType == typeof (string))
                                {
                                    value = extension.Invoke(null, new[] { value });
                                }

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
                var extension = customExtensions?.GetMethod(identifier, new[] { type });
                if (extension != null)
                {
                    value = extension.Invoke(null, new[] { context });
                    return true;
                }

                var property = type.GetProperty(identifier);
                if (property != null)
                {
                    value = property.GetValue(context);
                    return true;
                }

                extension = standardExtensions.GetMethod(identifier, new[] { type });
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
