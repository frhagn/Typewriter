using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Language.Intellisense;

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

        private static string GetSummary(ISymbol symbol)
        {
            string summary = "";
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
                        summary = m.ReturnType.ToDisplayString() + " " + m.ToDisplayString();
                    }
                    else
                    {
                        summary = symbol.ToDisplayString();
                    }
                }

                summary = summary.Replace("__Typewriter.__Code.", string.Empty);

                string documentation = symbol.GetDocumentationCommentXml();
                if (!string.IsNullOrEmpty(documentation))
                {
                    if (documentation.Contains("<summary>"))
                    {
                        try
                        {
                            summary += Environment.NewLine + (from item in XDocument.Parse("<r>" + documentation + "</r>").Descendants("summary")
                                                              select GetValue(item)
                                ).First().Trim();
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else
                    {
                        summary += Environment.NewLine + documentation.Trim();
                    }
                }
            }
            return summary;
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