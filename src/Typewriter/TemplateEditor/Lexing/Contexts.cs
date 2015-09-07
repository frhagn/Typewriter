using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Typewriter.CodeModel;
using Typewriter.CodeModel.Attributes;
using Typewriter.TemplateEditor.Lexing.Roslyn;
using Type = System.Type;

namespace Typewriter.TemplateEditor.Lexing
{
    public class Contexts
    {
        private readonly Dictionary<string, Context> items = new Dictionary<string, Context>();
        private readonly Dictionary<string, string> names = new Dictionary<string, string>();

        public Contexts(ShadowClass shadowClass)
        {
            var assembly = typeof(ContextAttribute).Assembly;

            ParseCodeModel(assembly);
            ParseExtensions(shadowClass);
        }

        public Context Find(string name)
        {
            if (items.ContainsKey(name) == false)
            {
                if (names.ContainsKey(name) == false)
                    return null;

                name = names[name];
            }

            return items[name];
        }

        private void ParseCodeModel(Assembly assembly)
        {
            var contexts = assembly.GetTypes().Where(t => t.IsDefined(typeof (ContextAttribute), false));

            foreach (var c in contexts)
            {
                var name = c.GetCustomAttribute<ContextAttribute>().Name;
                var collectionName = c.GetCustomAttribute<ContextAttribute>().CollectionName;
                var item = new Context(name, c);

                var properties = c.GetProperties();
                var inherited = c.GetInterfaces().SelectMany(i => i.GetProperties());

                foreach (var p in properties.Concat(inherited))
                {
                    var identifier = CreateIdentifier(p);
                    item.AddIdentifier(identifier);
                }

                names.Add(collectionName, name);
                items.Add(name, item);
            }
        }

        private void ParseExtensions(ShadowClass shadowClass)
        {
            foreach (var assembly in shadowClass.ReferencedAssemblies)
            {
                var methods = assembly.GetExportedTypes().SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .Where(m => m.IsDefined(typeof (ExtensionAttribute), false) &&
                        m.GetParameters().First().ParameterType.Namespace == "Typewriter.CodeModel"));

                foreach (var method in methods)
                {
                    var parameters = method.GetParameters();
                    if (parameters.Count() != 1) continue;

                    var context = items.Values.FirstOrDefault(c => c.Type == parameters.First().ParameterType);

                    if (context != null)
                    {
                        var identifier = CreateIdentifier(method);
                        context.AddExtensionIdentifier(method.ReflectedType.Namespace, identifier);
                    }
                }
            }
        }

        private Identifier CreateIdentifier(MemberInfo memberInfo)
        {
            var identifier = Identifier.FromMemberInfo(memberInfo);
            var type = GetType(memberInfo);

            var context = type.GetInterfaces().FirstOrDefault()?.GenericTypeArguments.FirstOrDefault();
            if (context?.GetCustomAttribute<ContextAttribute>() != null)
            {
                var contextName = context.GetCustomAttribute<ContextAttribute>().Name;
                identifier.Context = contextName;
                identifier.IsCollection = true;
                identifier.RequireTemplate = type.GetInterface(nameof(IStringConvertable)) == null;
            }

            if (type == typeof(bool))
            {
                identifier.IsBoolean = true;
            }
            else if (memberInfo.Name == "Parent")
            {
                identifier.IsParent = true;
            }
            else if (type.GetCustomAttribute<ContextAttribute>() != null)
            {
                var ctxn = type.GetCustomAttribute<ContextAttribute>().Name;
                identifier.Context = ctxn;
                identifier.HasContext = true;
            }

            return identifier;
        }

        private static Type GetType(MemberInfo memberInfo)
        {
            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null) return propertyInfo.PropertyType;

            var methodInfo = memberInfo as MethodInfo;
            if (methodInfo != null) return methodInfo.ReturnType;

            return null;
        }
    }
}