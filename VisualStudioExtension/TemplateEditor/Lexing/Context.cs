using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Typewriter.CodeModel;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.TemplateEditor.Lexing
{
    public class Context 
    {
        public Context(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public readonly Dictionary<string, Identifier> identifiers = new Dictionary<string, Identifier>();
        
        public Identifier GetIdentifier(string name)
        {
            Identifier i;
            return name != null && identifiers.TryGetValue(name, out i) ? i : null;
        }

        public ICollection<Identifier> Identifiers
        {
            get { return identifiers.Values.OrderBy(i => i.Name).ToArray(); }
        }
    }

    public class ContextSpan
    {
        public ContextSpan(int start, int end, Context context)
        {
            Start = start;
            End = end;
            Context = context;
        }

        public int Start { get; private set; }
        public int End { get; private set; }
        public Context Context { get; private set; }
    }

    public static class Contexts
    {
        private static readonly Dictionary<string, Context> items = new Dictionary<string, Context>();

        public static Context Find(string name)
        {
            return items[name];
        }

        static Contexts()
        {
            var contexts = typeof(ContextAttribute).Assembly.GetTypes().Where(t => CustomAttributeExtensions.GetCustomAttribute<ContextAttribute>((MemberInfo)t) != null);

            foreach (var c in contexts)
            {
                var name = c.GetCustomAttribute<ContextAttribute>().Name;
                var item = new Context(name);

                var properties = c.GetProperties().Where(p => p.GetCustomAttribute<PropertyAttribute>() != null);
                var inherited = c.GetInterfaces().SelectMany(i => i.GetProperties().Where(p => p.GetCustomAttribute<PropertyAttribute>() != null));

                foreach (var p in properties.Concat(inherited))
                {
                    var pname = p.GetCustomAttribute<PropertyAttribute>().Name;
                    var pdesc = p.GetCustomAttribute<PropertyAttribute>().Description.Replace("$context", name.ToLower());

                    var identifier = new Identifier { Name = p.Name, QuickInfo = pname + "\r\n" + pdesc };

                    var ctx = p.PropertyType.GenericTypeArguments.FirstOrDefault();
                    if (ctx != null && ctx.GetCustomAttribute<ContextAttribute>() != null)
                    {
                        var ctxn = ctx.GetCustomAttribute<ContextAttribute>().Name;
                        identifier.Context = ctxn;
                        identifier.IsCollection = true;
                    }

                    if (p.PropertyType == typeof(bool))
                    {
                        identifier.IsBoolean = true;
                    }

                    item.identifiers.Add(p.Name, identifier);
                }

                items.Add(name, item);
            }
        }
    }
}