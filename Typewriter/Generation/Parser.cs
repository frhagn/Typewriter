using System;
using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.CodeDom;
using Typewriter.TemplateEditor.Lexing;

namespace Typewriter.Generation
{
    public class Parser
    {
        private static readonly Type extensions = typeof(Extensions);

        public string Parse(string template, object context)
        {
            if (template == null) return null;

            var match = false;
            var output = string.Empty;
            var stream = new Stream(template);

            while (stream.Advance())
            {
                if (ParseDollar(stream, context, ref output, ref match)) continue;
                output += stream.Current;
            }

            return match ? output : null;
        }

        private string Parse(string template, object context, ref bool match)
        {
            if (template == null) return null;

            var output = string.Empty;
            var stream = new Stream(template);

            while (stream.Advance())
            {
                if (ParseDollar(stream, context, ref output, ref match)) continue;
                output += stream.Current;
            }

            return output;
        }

        private bool ParseDollar(Stream stream, object context, ref string output, ref bool match)
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

                        var items = Filter(collection, filter, ref match);
                        var mtch = match;
                        output += string.Join(Parse(separator, context, ref match), items.Select(item => Parse(block, item, ref mtch)));
                    }
                    else if (value is bool)
                    {
                        var trueBlock = ParseBlock(stream, '[', ']');
                        var falseBlock = ParseBlock(stream, '[', ']');

                        output += Parse((bool)value ? trueBlock : falseBlock, context, ref match);
                    }
                    else
                    {
                        var block = ParseBlock(stream, '[', ']');
                        if (block != null)
                        {
                            output += Parse(block, value, ref match);
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

        private static IEnumerable<object> Filter(IEnumerable<object> items, string filter, ref bool match)
        {
            if (string.IsNullOrWhiteSpace(filter)) return items;

            Func<ItemInfo, IEnumerable<string>> selector;

            filter = filter.Trim();

            if (filter.StartsWith("[") && filter.EndsWith("]"))
            {
                filter = filter.Trim('[', ']', ' ');
                selector = i => i.Attributes.SelectMany(a => new[] { a.Name, a.FullName });
            }
            else if (filter.StartsWith(":"))
            {
                filter = filter.Remove(0, 1).Trim();
                selector = i => i.Interfaces.SelectMany(a => new[] { a.Name, a.FullName });
            }
            else
            {
                selector = i => new[] { i.Name, i.FullName };
            }

            var parts = filter.Split('*');

            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];

                if (parts.Length == 1)
                {
                    items = items.Cast<ItemInfo>().Where(item => selector(item).Any(p => p == part));
                }
                else if (i == 0 && string.IsNullOrWhiteSpace(part) == false)
                {
                    items = items.Cast<ItemInfo>().Where(item => selector(item).Any(p => p.StartsWith(part)));
                }
                else if (i == parts.Length - 1 && string.IsNullOrWhiteSpace(part) == false)
                {
                    items = items.Cast<ItemInfo>().Where(item => selector(item).Any(p => p.EndsWith(part)));
                }
                else if (i > 0 && i < parts.Length - 1 && string.IsNullOrWhiteSpace(part) == false)
                {
                    items = items.Cast<ItemInfo>().Where(item => selector(item).Any(p => p.Contains(part)));
                }
            }

            var filtered = items.ToList();
            match = match || filtered.Any();

            return filtered;
        }

        private static object GetIdentifier(string identifier, object context)
        {
            if (identifier == null) return null;

            var type = context.GetType();

            var extension = extensions.GetMethod(identifier, new[] { type });
            if (extension != null)
            {
                return extension.Invoke(null, new[] { context });
            }

            var property = type.GetProperty(identifier);
            if (property != null)
            {
                return property.GetValue(context);
            }

            return null;
        }
    }
}
