using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Typewriter.CodeModel;
using Typewriter.CodeModel.Attributes;
using Typewriter.Generation;
using Type = System.Type;

namespace Typewriter.TemplateEditor.Lexing
{
    public static class Contexts
    {
        private static readonly Type extensions = typeof(Extensions);
        private static readonly Dictionary<string, Context> items = new Dictionary<string, Context>();
        private static readonly Dictionary<string, string> names = new Dictionary<string, string>();

        public static Context Find(string name)
        {
            if (items.ContainsKey(name) == false)
            {
                if (names.ContainsKey(name) == false)
                    return null;

                name = names[name];
            }

            return items[name];
        }

        static Contexts()
        {
            var contexts = typeof(ContextAttribute).Assembly.GetTypes().Where(t => t.GetCustomAttribute<ContextAttribute>() != null);

            foreach (var c in contexts)
            {
                var name = c.GetCustomAttribute<ContextAttribute>().Name;
                var collectionName = c.GetCustomAttribute<ContextAttribute>().CollectionName;
                var item = new Context(name, c);
                
                var properties = c.GetProperties().Where(p => p.GetCustomAttribute<PropertyAttribute>() != null);
                var inherited = c.GetInterfaces().SelectMany(i => i.GetProperties().Where(p => p.GetCustomAttribute<PropertyAttribute>() != null));

                foreach (var p in properties.Concat(inherited))
                {
                    var propertyName = p.GetCustomAttribute<PropertyAttribute>().Name;
                    var propertyDescription = p.GetCustomAttribute<PropertyAttribute>().Description.Replace("$context", name.ToLower());
                    var identifier = new Identifier { Name = p.Name, QuickInfo = propertyName + Environment.NewLine + propertyDescription };
                    var ctx = p.PropertyType.GetInterfaces().FirstOrDefault()?.GenericTypeArguments.FirstOrDefault();

                    if (ctx?.GetCustomAttribute<ContextAttribute>() != null)
                    {
                        var ctxn = ctx.GetCustomAttribute<ContextAttribute>().Name;
                        identifier.Context = ctxn;
                        identifier.IsCollection = true;
                    }
                    
                    if (p.PropertyType == typeof(bool))
                    {
                        identifier.IsBoolean = true;
                    }
                    else if (p.Name == "Parent")
                    {
                        identifier.IsParent = true;
                    }
                    else if (p.PropertyType.GetCustomAttribute<ContextAttribute>() != null)
                    {
                        var ctxn = p.PropertyType.GetCustomAttribute<ContextAttribute>().Name;
                        identifier.Context = ctxn;
                        identifier.HasContext = true;
                    }

                    item.AddIdentifier(identifier);
                }

                var methods = extensions.GetMethods().Where(m => m.GetCustomAttribute<PropertyAttribute>() != null && m.GetParameters().All(p => p.ParameterType == c));
                var inheritedMethods = c.GetInterfaces().SelectMany(i => extensions.GetMethods().Where(m => m.GetCustomAttribute<PropertyAttribute>() != null && m.GetParameters().All(p => p.ParameterType == i)));
                
                foreach (var m in methods.Concat(inheritedMethods))
                {
                    var methodName = m.GetCustomAttribute<PropertyAttribute>().Name;
                    var methodDescription = m.GetCustomAttribute<PropertyAttribute>().Description.Replace("$context", name.ToLower());
                    var identifier = new Identifier { Name = m.Name, QuickInfo = methodName + Environment.NewLine + methodDescription };
                    var ctx = m.ReturnType.GetInterfaces().FirstOrDefault()?.GenericTypeArguments.FirstOrDefault();

                    if (ctx?.GetCustomAttribute<ContextAttribute>() != null)
                    {
                        var ctxn = ctx.GetCustomAttribute<ContextAttribute>().Name;
                        identifier.Context = ctxn;
                        identifier.IsCollection = true;
                    }

                    if (m.ReturnType == typeof(bool))
                    {
                        identifier.IsBoolean = true;
                    }
                    else if (m.ReturnType.GetCustomAttribute<ContextAttribute>() != null)
                    {
                        var ctxn = m.ReturnType.GetCustomAttribute<ContextAttribute>().Name;
                        identifier.Context = ctxn;
                        identifier.HasContext = true;
                    }

                    item.AddIdentifier(identifier);
                }

                names.Add(collectionName, name);
                items.Add(name, item);
            }
        }
    }
}