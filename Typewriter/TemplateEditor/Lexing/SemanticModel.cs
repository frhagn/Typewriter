using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Typewriter.TemplateEditor.Lexing.Roslyn;

namespace Typewriter.TemplateEditor.Lexing
{
    public class SemanticModel
    {
        private readonly ShadowClass shadowClass;
        private readonly Tokens tokens = new Tokens();
        private readonly ContextSpans contextSpans = new ContextSpans();      
        private readonly Identifiers identifiers = new Identifiers();

        public Tokens Tokens => tokens;
        public ContextSpans ContextSpans => contextSpans;
        public Identifiers Identifiers => identifiers;
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
                    return contextIdentifiers.Concat(this.identifiers.GetTempIdentifiers(contextSpan.Context)).OrderBy(i => i.Name);
                }

                return shadowClass.GetRecommendedSymbols(position).GroupBy(s => s.Name).Select(g => Identifier.FromSymbol(g.First())).OrderBy(i => i.Name).ToList();
            }

            return new Identifier[0];
        }

        // Lexers
        internal Identifier GetIdentifier(Context context, string name)
        {
            var identifier = context.GetIdentifier(name);
            return identifier ?? identifiers.GetTempIdentifiers(context).FirstOrDefault(i => i.Name == name);
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