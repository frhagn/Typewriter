using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Typewriter.TemplateEditor.Lexing.Roslyn;

namespace Typewriter.TemplateEditor.Lexing
{
    public class SemanticModel
    {
        private readonly ShadowClass shadowClass;
        private readonly Tokens tokens = new Tokens();
        private readonly ContextSpans contextSpans = new ContextSpans();      
        private readonly Identifiers tempIdentifiers = new Identifiers();

        public Tokens Tokens => tokens;
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
                    return contextIdentifiers.Concat(this.tempIdentifiers.GetTempIdentifiers(contextSpan.Context)).OrderBy(i => i.Name);
                }

                var identifiers = shadowClass.GetRecommendedSymbols(position).GroupBy(s => s.Name).Select(g => Identifier.FromSymbol(g.First())).ToList();

                // Add common keywords to the statement completion list. (Roslyn 1.1 might provide this funtionality)
                if (identifiers.Any(i => i.Name == "Boolean") && identifiers.Any(i => i.Name == "Class"))
                {
                    identifiers.AddRange(new[]
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

                        new Identifier { Name = "new", QuickInfo = "new Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
                        new Identifier { Name = "return", QuickInfo = "return Keyword", Glyph = StandardGlyphGroup.GlyphKeyword },
                        new Identifier { Name = "using", QuickInfo = "using Keyword", Glyph = StandardGlyphGroup.GlyphKeyword }
                    });
                }

                return identifiers.OrderBy(i => i.Name);
            }

            return new Identifier[0];
        }

        // Lexers
        internal Identifier GetIdentifier(Context context, string name)
        {
            var identifier = context.GetIdentifier(name);
            return identifier ?? tempIdentifiers.GetTempIdentifiers(context).FirstOrDefault(i => i.Name == name);
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
                return tokens.GetToken(position)?.QuickInfo;
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