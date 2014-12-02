using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Typewriter.TemplateEditor.Lexing;
using Typewriter.TemplateEditor.Lexing.Tokens;

namespace Typewriter.TemplateEditor
{
    public class Editor
    {
        private readonly Lexer lexer = new Lexer();
        private static readonly Editor instance = new Editor();

        public static Editor Instance
        {
            get { return instance; }
        }

        private ITextSnapshot snapshot;
        private TokenList tokenList;

        private TokenList GetTokens(ITextBuffer buffer)
        {
            if (snapshot == buffer.CurrentSnapshot)
                return tokenList;

            snapshot = buffer.CurrentSnapshot;
            tokenList = lexer.Tokenize(snapshot.GetText());

            return tokenList;
        }

        // Brace matching
        public IEnumerable<ITagSpan<TextMarkerTag>> GetBraceTags(ITextBuffer buffer, SnapshotSpan span)
        {
            var tokens = GetTokens(buffer);

            var token = tokens.FirstOrDefault(t => t.Start == span.Start && t.Length == 1);// Här!
            if (token != null && token.MatchingToken != null)
            {
                var tag = new TextMarkerTag(Classifications.BraceMatching);
                var matchingSpan = new SnapshotSpan(span.Snapshot, token.MatchingToken.Start, 1);

                yield return new TagSpan<TextMarkerTag>(span, tag);
                yield return new TagSpan<TextMarkerTag>(matchingSpan, tag);
            }
        }

        // Classification
        public IEnumerable<ClassificationSpan> GetClassificationSpans(ITextBuffer buffer, SnapshotSpan span, IClassificationTypeRegistryService classificationRegistry)
        {
            var tokens = GetTokens(buffer);


            foreach (var token in tokens)
            {
                if (token.Start > span.End)
                    break;

                if (token.Start <= span.End && token.End >= span.Start && token.Classification != null)// Här!
                {
                    var classificationType = classificationRegistry.GetClassificationType(token.Classification);
                    yield return new ClassificationSpan(new SnapshotSpan(span.Snapshot, token.Start, token.Length), classificationType);
                }
            }
        }

        // Statement completion
        public IEnumerable<Completion> GetCompletions(ITextBuffer buffer, SnapshotSpan span, IGlyphService glyphService)
        {
            var tokens = GetTokens(buffer);
            var imageSource = glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupProperty, StandardGlyphItem.GlyphItemPublic);

            var token = tokens.LastOrDefault(t => t.Start <= span.Start); // Här!
            if (token != null)
            {
                return token.Context.Identifiers.Select(i => new Completion(i.Name, i.Name, i.QuickInfo, imageSource, null));
            }

            return new Completion[0];
        }

        // Quick info
        public string GetQuickInfo(ITextBuffer buffer, SnapshotSpan span)
        {
            var tokens = GetTokens(buffer);
            var token = tokens.FirstOrDefault(t => t.Start == span.Start && t.End == span.End);

            if (token != null && token.QuickInfo != null)
            {
                return token.QuickInfo;
            }

            return null;
        }
    }
}