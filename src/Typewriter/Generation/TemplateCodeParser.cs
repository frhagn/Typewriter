using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using EnvDTE;
using Typewriter.Generation.Controllers;
using Typewriter.TemplateEditor.Lexing;
using Typewriter.TemplateEditor.Lexing.Roslyn;
using Typewriter.VisualStudio;
using Stream = Typewriter.TemplateEditor.Lexing.Stream;

namespace Typewriter.Generation
{
    public static class TemplateCodeParser
    {
        private static int counter;

        public static string Parse(ProjectItem templateProjectItem, string template, List<Type> extensions)
        {
            if (string.IsNullOrWhiteSpace(template)) return null;

            var output = string.Empty;
            var stream = new Stream(template);
            var shadowClass = new ShadowClass();
            var contexts = new Contexts(shadowClass);

            shadowClass.Clear();

            while (stream.Advance())
            {
                if (ParseReference(stream, shadowClass, templateProjectItem)) continue;
                if (ParseCodeBlock(stream, shadowClass)) continue;
                if (ParseLambda(stream, shadowClass, contexts, ref output)) continue;
                output += stream.Current;
            }

            shadowClass.Parse();

            extensions.Clear();
            extensions.Add(Compiler.Compile(templateProjectItem, shadowClass));
            extensions.AddRange(FindExtensionClasses(shadowClass));

            return output;
        }

        private static IEnumerable<Type> FindExtensionClasses(ShadowClass shadowClass)
        {
            var types = new List<Type>();

            var usings = shadowClass.Snippets.Where(s => s.Type == SnippetType.Using && s.Code.StartsWith("using"));
            foreach (var usingStatement in usings.Select(u => u.Code))
            {
                var ns = usingStatement.Remove(0, 5).Trim().Trim(';');

                foreach (var assembly in shadowClass.ReferencedAssemblies)
                {
                    types.AddRange(assembly.GetExportedTypes().Where(t => t.Namespace == ns &&
                        t.GetMethods(BindingFlags.Static | BindingFlags.Public).Any(m => 
                            m.IsDefined(typeof (ExtensionAttribute), false) && 
                            m.GetParameters().First().ParameterType.Namespace == "Typewriter.CodeModel")));
                }
            }

            return types;
        }

        private static bool ParseCodeBlock(Stream stream, ShadowClass shadowClass)
        {
            if (stream.Current == '$' && stream.Peek() == '{')
            {
                for (var i = 0; ; i--)
                {
                    var current = stream.Peek(i);
                    if (current == '`' || (current == '/' && stream.Peek(i-1) == '/')) return false;
                    if (current == '\n' || current == char.MinValue) break;
                }

                stream.Advance();

                var block = stream.PeekBlock(1, '{', '}');
                var codeStream = new Stream(block, stream.Position + 1);

                ParseUsings(codeStream, shadowClass);
                ParseCode(codeStream, shadowClass);

                stream.Advance(block.Length + 1);

                return true;
            }

            return false;
        }

        private static void ParseUsings(Stream stream, ShadowClass shadowClass)
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

        private static void ParseCode(Stream stream, ShadowClass shadowClass)
        {
            var code = new StringBuilder();

            do
            {
                if (stream.Current != char.MinValue)
                    code.Append(stream.Current);
            }
            while (stream.Advance());

            shadowClass.AddBlock(code.ToString(), 0);
        }

        private static bool ParseLambda(Stream stream, ShadowClass shadowClass, Contexts contexts, ref string template)
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

                                var contextName = identifier;
                                // Todo: Make the TemplateCodeParser context aware
                                if (contextName == "TypeArguments") contextName = "Types";
                                else if (contextName.StartsWith("Nested")) contextName = contextName.Remove(0, 6);

                                var type = contexts.Find(contextName)?.Type.FullName;

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

        private static bool ParseReference(Stream stream, ShadowClass shadowClass, ProjectItem templateProjectItem)
        {
            const string keyword = "reference";

            if (stream.Current == '#' && stream.Peek() == keyword[0] && stream.PeekWord(1) == keyword)
            {
                var reference = stream.PeekLine(keyword.Length + 1);
                if (reference != null)
                {
                    var len = reference.Length + keyword.Length + 1;
                    reference = reference.Trim('"', ' ', '\n', '\r');
                    try
                    {
                        if (reference.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                            reference = PathResolver.ResolveRelative(reference, templateProjectItem);

                        shadowClass.AddReference(reference);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Reference Error: " + ex);
                    }
                    finally
                    {
                        stream.Advance(len - 1);
                    }
                }
            }

            return false;
        }
    }
}
