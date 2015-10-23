using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Language.Intellisense;
using Typewriter.TemplateEditor.Lexing.Roslyn;

namespace Typewriter.TemplateEditor.Lexing
{
    public class Identifier
    {
        public string Name { get; set; }
        public string QuickInfo { get; set; }
        public string Context { get; set; }
        public bool IsCollection { get; set; }
        public bool IsBoolean { get; set; }
        public bool HasContext { get; set; }
        public bool IsParent { get; set; }
        public bool RequireTemplate { get; set; }
        public bool IsCustom { get; set; }
        public StandardGlyphGroup Glyph { get; set; } = StandardGlyphGroup.GlyphGroupProperty;

        public static Identifier FromSymbol(ISymbol symbol)
        {
            return new Identifier
            {
                Name = symbol.Name,
                QuickInfo = GetSummary(symbol),
                Glyph = symbol.Kind == SymbolKind.Method ? StandardGlyphGroup.GlyphGroupMethod : StandardGlyphGroup.GlyphGroupProperty
            };
        }

        public static Identifier FromMemberInfo(MemberInfo memberInfo)
        {
            var assembly = memberInfo.ReflectedType.Assembly;
            var documentationProvider = XmlDocumentationProvider.GetDocumentationProvider(assembly);

            var name = memberInfo.Name;
            string documentation = null;

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                documentation = string.Concat(GetTypeName(propertyInfo.PropertyType.Name), " ", name, 
                    ParseDocumentation(documentationProvider.GetDocumentationForSymbol("P:" + propertyInfo.ReflectedType.FullName + "." + propertyInfo.Name)));
            }
            else
            {
                var methodInfo = memberInfo as MethodInfo;
                if (methodInfo != null)
                {
                    var prefix = methodInfo.IsDefined(typeof (ExtensionAttribute), false) ? "(extension) " : "";
                    var parameters = string.Join(",", methodInfo.GetParameters().Select(p => p.ParameterType.FullName));
                    var typName = $"M:{methodInfo.ReflectedType.FullName}.{methodInfo.Name}({parameters})";

                    documentation = string.Concat(prefix, GetTypeName(methodInfo.ReturnType.Name), " ", name,
                        ParseDocumentation(documentationProvider.GetDocumentationForSymbol(typName)));
                }
            }

            return new Identifier
            {
                Name = name,
                QuickInfo = documentation
            };
        }

        private static string GetTypeName(string name)
        {
            if (name == "String") return "string";
            if (name == "Boolean") return "bool";

            return name;
        }

        private static string GetSummary(ISymbol symbol)
        {
            var summary = "";

            if (symbol != null)
            {
                var p = symbol as IPropertySymbol;
                if (p != null)
                {
                    summary = p.Type.ToDisplayString() + " " + p.Name;
                }
                else
                {
                    var m = symbol as IMethodSymbol;
                    if (m != null)
                    {
                        var prefix = m.IsExtensionMethod ? "(extension) " : "";
                        summary = string.Concat(prefix, m.ReturnType.ToDisplayString(), " ", m.ToDisplayString());
                    }
                    else
                    {
                        summary = symbol.ToDisplayString();
                    }
                }

                summary = summary.Replace("__Typewriter.", string.Empty);
                //summary = summary.Replace("__Code.", string.Empty);

                var documentation = symbol.GetDocumentationCommentXml();
                summary += ParseDocumentation(documentation);
            }

            return summary;
        }

        private static string ParseDocumentation(string documentation)
        {
            if (string.IsNullOrEmpty(documentation) == false)
            {
                if (documentation.Contains("<summary>"))
                {
                    try
                    {
                        documentation = XDocument.Parse("<r>" + documentation + "</r>").Descendants("summary").Select(GetValue).FirstOrDefault() ?? string.Empty;
                        documentation = Regex.Replace(documentation, @"^ +", "", RegexOptions.Multiline);

                        if (Constants.RoslynEnabled)
                        {
                            documentation = Regex.Replace(documentation, @"^\s*\(In Visual Studio 2013.*", "", RegexOptions.Multiline);
                        }
                    }
                    catch
                    {
                    }
                }

                return Environment.NewLine + documentation.Trim();
            }

            return string.Empty;
        }

        private static string GetValue(XElement element)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var node in element.Nodes())
            {
                var textNode = node as XText;
                if (textNode != null)
                {
                    stringBuilder.Append(textNode.Value);
                }
                var elementNode = node as XElement;
                if (elementNode != null && elementNode.Name == "see")
                {
                    var crefAttribute = elementNode.Attributes("cref").FirstOrDefault();
                    if (crefAttribute != null)
                    {
                        stringBuilder.Append(crefAttribute.Value.Split('.').Last());
                    }
                }
            }
            return stringBuilder.ToString();
        }
    }

    public class TemporaryIdentifier
    {
        public TemporaryIdentifier(Context context, Identifier identifier)
        {
            Context = context;
            Identifier = identifier;
        }

        public Context Context { get; set; }
        public Identifier Identifier { get; set; }
    }
}