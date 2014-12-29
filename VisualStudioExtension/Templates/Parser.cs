using System;
using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.CodeDom;

namespace Typewriter.Templates
{
    public class Parser
    {
        private readonly Type extensions;
        private bool match;

        public Parser(Type extensions)
        {
            this.extensions = extensions;
        }

        public string Parse(string template, object data)
        {
            var isKeyword = false;
            string keyword = null;
            string filter = null;

            string output = null;

            for (var i = 0; i < template.Length; i++)
            {
                var current = template[i];

                if (isKeyword)
                {
                    if (current == '<')
                    {
                        filter = ParseFilter(ref i, template);
                        continue;
                    }

                    if (current == '[')
                    {
                        output += ParseBlock(ref i, template, keyword, filter, data);
                        isKeyword = false;
                        continue;
                    }

                    if (current == '}')
                    {
                        output += GetValue(keyword, data);
                        match = true;
                        isKeyword = false;
                        continue;
                    }

                    keyword += current;
                }
                else
                {
                    if (current == '$' && i < template.Length - 1 && template[i + 1] == '{')
                    {
                        i++;
                        isKeyword = true;
                        keyword = null;
                        filter = null;
                        continue;
                    }

                    output += current;
                }
            }

            return match ? output : null;
        }

        private static string ParseFilter(ref int i, string template)
        {
            i++;
            int count = 1;
            string filter = null;

            for (; i < template.Length; i++)
            {
                var current = template[i];
                if (current == '<') count++;

                if (current == '>')
                {
                    if (count == 1) return filter;
                    count--;
                }

                filter += current;
            }

            throw new Exception("Filter");
        }

        private string ParseBlock(ref int i, string template, string keyword, string filter, object data)
        {
            i++;
            int count = 1;
            string block = null;
            string separator = null;

            for (; i < template.Length; i++)
            {
                var current = template[i];
                if (current == '[')count++;
                if (current == ']')
                {
                    if (count == 1)
                    {
                        if (i < template.Length - 1 && template[i + 1] == '[')
                        {
                            i++;
                            separator = ParseSeparator(template, ref i);
                        }

                        if (i < template.Length - 1 && template[i + 1] == '}')
                        {
                            i++;
                        }

                        return GetBlockValue(block, keyword, filter, data, separator);
                    }
                    count--;
                }

                block += current;
            }

            throw new Exception("Block");
        }

        private static string ParseSeparator(string template, ref int i)
        {
            i++;
            int count = 1;
            string separator = null;

            for (; i < template.Length; i++)
            {
                var current = template[i];
                if (current == '[') count++;
                if (current == ']')
                {
                    if (count == 1 && i < template.Length - 1 && template[i + 1] == '}') return separator;
                    count--;
                }

                separator += current;
            }

            throw new Exception("Separator");
        }

        private string GetBlockValue(string block, string keyword, string filter, object data, string separator)
        {
            var value = GetValue(keyword, data);
            var list = value as IEnumerable<object>;

            if (list != null)
            {
                if (filter != null)
                {
                    if (filter.StartsWith("\"")) return Filter(list.Cast<ItemInfo>(), m => new [] { m.Name, m.FullName }, filter.Replace("\"", ""), block, separator);
                    if (filter.StartsWith(":")) return Filter(list.Cast<ItemInfo>(), m => m.Interfaces.SelectMany(i => new[] { i.Name, i.FullName }), filter.Replace(":", ""), block, separator);
                    if (filter.StartsWith("[")) return Filter(list.Cast<ItemInfo>(), m => m.Attributes.SelectMany(i => new[] { i.Name, i.FullName }), filter.Replace("[", "").Replace("]", ""), block, separator);
                    
                    // Returvärdet
                    if (filter.StartsWith("=\"")) return Filter(list.Cast<ItemInfo>(), m => new[] { m.Type.Name, m.Type.FullName }, filter.Replace("=\"", ""), block, separator);
                    if (filter.StartsWith("=:")) return Filter(list.Cast<ItemInfo>(), m => m.Type.Interfaces.SelectMany(i => new[] { i.Name, i.FullName }), filter.Replace("=:", ""), block, separator);
                    if (filter.StartsWith("=[")) return Filter(list.Cast<ItemInfo>(), m => m.Type.Attributes.SelectMany(i => new[] { i.Name, i.FullName }), filter.Replace("=[", "").Replace("]", ""), block, separator);

                    return MethodFilter(list.Cast<ItemInfo>(), filter, block, separator);
                }

                return string.Join(separator, list.Select(y =>
                {
                    match = true;
                    return block == null ? null : Parse(block, y);
                }));
            }

            if (value is bool)
            {
                match = true;

                if ((bool)value)
                {
                    return block == null ? null : Parse(block, data);
                }

                return separator != null ? Parse(separator, data) : null;
            }

            return block == null ? null : Parse(block, value);
        }

        private string Filter(IEnumerable<ItemInfo> items, Func<ItemInfo, IEnumerable<string>> property, string filter, string block, string separator)
        {
            if (items == null)
            {
                return null;
            }

            var parts = filter.Split('*');
            
            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];

                if (parts.Length == 1)
                {
                    items = items.Where(z => property(z).Any(p => p == part));
                }
                else if (i == 0 && string.IsNullOrWhiteSpace(part) == false)
                {
                    items = items.Where(z => property(z).Any(p => p.StartsWith(part)));
                }
                else if (i == parts.Length - 1 && string.IsNullOrWhiteSpace(part) == false)
                {
                    items = items.Where(z => property(z).Any(p => p.EndsWith(part)));
                }
                else if (i > 0 && i < parts.Length - 1 && string.IsNullOrWhiteSpace(part) == false)
                {
                    items = items.Where(z => property(z).Any(p => p.Contains(part)));
                }
            }

            return string.Join(separator, items.Select(y =>
            {
                match = true;
                return block == null ? null : Parse(block, y);
            }));
        }

        private string MethodFilter(IEnumerable<ItemInfo> items, string filter, string block, string separator)
        {
            if (items == null) return null;
            var list = items.ToList();
            if (list.Any() == false) return null;

            var type = list.First().GetType();
            var method = extensions.GetMethod(filter, new[] { type });
            if (method == null)
            {
                method = typeof(SolutionExtensions).GetMethod(filter, new[] { type });
                if (method == null)
                {
                    return string.Format("<Can't find method {0}({1})>", filter, type);
                }
            }

            return string.Join(separator, list.Where(item => (bool)method.Invoke(null, new object[] { item })).Select(y =>
            {
                match = true;
                return block == null ? null : Parse(block, y);
            }));
        }

        private object GetValue(string keyword, object data)
        {
            while (true)
            {
                var index = keyword.IndexOf(".", StringComparison.InvariantCultureIgnoreCase);

                if (index > -1)
                {
                    data = GetValue(keyword.Substring(0, index), data);
                    keyword = keyword.Substring(index + 1);
                    continue;
                }
                break;
            }

            var type = data.GetType();

            // Template extensions
            var method = extensions.GetMethod(keyword, new[] { type });
            if (method != null)
            {
                return method.Invoke(null, new[] { data });
            }

            // Normal
            var property = type.GetProperty(keyword);
            if (property != null)
            {
                return property.GetValue(data);
            }

            // Auto camel case
            property = type.GetProperty(PascalCase(keyword));
            if (property != null)
            {
                var value = property.GetValue(data);
                return value == null ? null : CamelCase(value.ToString());
            }

            // Typewriter extensions
            method = typeof(Extensions).GetMethod(keyword, new[] { type });
            if (method != null)
            {
                return method.Invoke(null, new[] { data });
            }

            return string.Format("<Can't find property or method {0} on {1}>", keyword, type);
        }

        private static string CamelCase(string value)
        {
            if (value == null) return null;
            if (value.Length <= 1) return value.ToLowerInvariant();

            return value.Substring(0, 1).ToLowerInvariant() + value.Substring(1);
        }

        private static string PascalCase(string value)
        {
            if (value == null) return null;
            if (value.Length <= 1) return value.ToUpperInvariant();

            return value.Substring(0, 1).ToUpperInvariant() + value.Substring(1);
        }
    }
}
