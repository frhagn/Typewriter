using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Typewriter.TemplateEditor.Lexing.Roslyn;

namespace Typewriter.TemplateEditor.Lexing
{
    public class SemanticModel
    {
        #region Keywords

        private static readonly Identifier[] keywords = new[]
        {
            new Identifier { Name = "bool", QuickInfo = "bool Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "byte", QuickInfo = "byte Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "char", QuickInfo = "char Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "decimal", QuickInfo = "decimal Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "double", QuickInfo = "double Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "float", QuickInfo = "float Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "int", QuickInfo = "int Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "long", QuickInfo = "long Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "object", QuickInfo = "object Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "sbyte", QuickInfo = "sbyte Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "short", QuickInfo = "short Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "string", QuickInfo = "string Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "uint", QuickInfo = "uint Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "ulong", QuickInfo = "ulong Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "ushort", QuickInfo = "ushort Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "void", QuickInfo = "void Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },

            new Identifier { Name = "as", QuickInfo = "as Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "break", QuickInfo = "break Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "case", QuickInfo = "case Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            //new Identifier { Name = "class", QuickInfo = "class Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "const", QuickInfo = "const Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "continue", QuickInfo = "continue Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "do", QuickInfo = "do Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "else", QuickInfo = "else Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            //new Identifier { Name = "enum", QuickInfo = "enum Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "false", QuickInfo = "false Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "finally", QuickInfo = "finally Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "for", QuickInfo = "for Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "foreach", QuickInfo = "foreach Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "if", QuickInfo = "if Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "is", QuickInfo = "is Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "new", QuickInfo = "new Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "null", QuickInfo = "null Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "out", QuickInfo = "out Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "ref", QuickInfo = "ref Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "return", QuickInfo = "return Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "static", QuickInfo = "static Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "struct", QuickInfo = "struct Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "switch", QuickInfo = "switch Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "throw", QuickInfo = "throw Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "true", QuickInfo = "true Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "try", QuickInfo = "try Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "typeof", QuickInfo = "typeof Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "using", QuickInfo = "using Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "var", QuickInfo = "var Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
            new Identifier { Name = "while", QuickInfo = "while Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
        };

        #endregion

        private readonly ShadowClass shadowClass;
        private readonly Tokens tokens = new Tokens();
        private readonly Tokens errorTokens = new Tokens();
        private readonly ContextSpans contextSpans = new ContextSpans();      
        private readonly Identifiers tempIdentifiers = new Identifiers();

        public Tokens Tokens => tokens;
        public Tokens ErrorTokens => errorTokens;
        public ContextSpans ContextSpans => contextSpans;
        public Identifiers TempIdentifiers => tempIdentifiers;
        public ShadowClass ShadowClass => shadowClass;

        public SemanticModel(ShadowClass shadowClass)
        {
            this.shadowClass = shadowClass;
        }
        
        // Completion
        public IEnumerable<Identifier> GetIdentifiers(int position)
        {
            var contextSpan = contextSpans.GetContextSpan(position);
            if (contextSpan != null)
            {
                if (contextSpan.Type == ContextType.Template)
                {
                    var contextIdentifiers = contextSpan.Context.Identifiers;
                    var customIdentifiers = this.tempIdentifiers.GetTempIdentifiers(contextSpan.Context);
                    // Todo: Optimize performance
                    var extensionIdentifiers = shadowClass.Snippets.Where(s => s.Type == SnippetType.Using && s.Code.StartsWith("using"))
                        .SelectMany(s => contextSpan.Context.GetExtensionIdentifiers(s.Code.Remove(0, 5).Trim().TrimEnd(';')));

                    return contextIdentifiers.Concat(customIdentifiers).Concat(extensionIdentifiers).OrderBy(i => i.Name);
                }

                var identifiers = shadowClass.GetRecommendedSymbols(position).GroupBy(s => s.Name).Select(g => Identifier.FromSymbol(g.First())).ToList();

                // Add common keywords to the statement completion list. (Roslyn 1.1 might provide this funtionality)
                if (identifiers.Any(i => i.Name == "Boolean") && identifiers.Any(i => i.Name == "Class"))
                {
                    identifiers.AddRange(keywords);
                }

                return identifiers.OrderBy(i => i.Name);
            }

            return new Identifier[0];
        }

        // Lexers
        internal Identifier GetIdentifier(Context context, string name)
        {
            var identifier = context.GetIdentifier(name);
            if (identifier != null) return identifier;

            identifier = tempIdentifiers.GetTempIdentifiers(context).FirstOrDefault(i => i.Name == name);
            if (identifier != null) return identifier;

            // Todo: Optimize performance
            foreach (var snippet in shadowClass.Snippets.Where(s => s.Type == SnippetType.Using && s.Code.StartsWith("using")))
            {
                identifier = context.GetExtensionIdentifier(snippet.Code.Remove(0, 5).Trim().TrimEnd(';'), name);
                if (identifier != null) return identifier;
            }

            return null;
        }

        // BraceMatching
        public Token GetToken(int position)
        {
            return tokens.GetToken(position);
        }

        // QuickInfo
        public string GetQuickInfo(int position)
        {
            var contextSpan = contextSpans.GetContextSpan(position);
            if (contextSpan?.Type == ContextType.Template)
            {
                var quickInfo = tokens.GetToken(position)?.QuickInfo;
                if (quickInfo != null && quickInfo.StartsWith("Item Parent"))
                {
                    var parent = contextSpan.ParentContext?.Name;
                    if (parent != null)
                    {
                        quickInfo = parent + quickInfo.Remove(0, 4);
                    }
                }

                return quickInfo;
            }

            var error = errorTokens.FindTokens(position).FirstOrDefault();
            if (error != null)
            {
                return error.QuickInfo;
            }

            var symbol = shadowClass.GetSymbol(position);
            if (symbol != null)
            {
                return Identifier.FromSymbol(symbol).QuickInfo;
            }

            return null;
        }

        // Classification
        public IEnumerable<Token> GetTokens(Span span)
        {
            return tokens.GetTokens(span);
        }

        // Snytax errors
        public IEnumerable<Token> GetErrorTokens(Span span)
        {
            return errorTokens.GetTokens(span);
        }

        // Statement completion
        public ContextSpan GetContextSpan(int position)
        {
            return contextSpans.GetContextSpan(position);
        }

        // Outlining
        public IEnumerable<ContextSpan> GetContextSpans(ContextType type)
        {
            return contextSpans.GetContextSpans(type);
        }
    }
}