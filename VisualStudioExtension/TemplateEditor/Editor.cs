using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Typewriter.TemplateEditor.Lexing;

namespace Typewriter.TemplateEditor
{
    public class Editor
    {
        private static readonly Editor instance = new Editor();

        public static Editor Instance
        {
            get { return instance; }
        }

        private ITextSnapshot snapshot;
        private Lexer lexer;

        private Lexer GetTokens(ITextBuffer buffer)
        {
            if (snapshot == buffer.CurrentSnapshot)
                return lexer;

            snapshot = buffer.CurrentSnapshot;
            lexer = new Lexer(snapshot.GetText());

            return lexer;
        }

        // Brace matching
        public IEnumerable<ITagSpan<TextMarkerTag>> GetBraceTags(ITextBuffer buffer, SnapshotPoint point)
        {
            yield break;
            //var tokens = GetTokens(buffer);
            //var token = tokens.GetToken(point.Position - 1);
            //var tag = new TextMarkerTag(Classifications.BraceHighlight);

            //if (token != null && token.MatchingToken != null && token.Type.ToString().StartsWith("Close"))
            //{
            //    yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(point.Snapshot, token.Start, 1), tag);
            //    yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(point.Snapshot, token.MatchingToken.Start, 1), tag);
            //}
            //else
            //{
            //    token = tokens.GetToken(point.Position);

            //    if (token != null && token.MatchingToken != null && token.Type.ToString().StartsWith("Open"))
            //    {
            //        yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(point.Snapshot, token.Start, 1), tag);
            //        yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(point.Snapshot, token.MatchingToken.Start, 1), tag);
            //    }
            //}
        }

        // Classification
        public IEnumerable<ClassificationSpan> GetClassificationSpans(ITextBuffer buffer, SnapshotSpan span, IClassificationTypeRegistryService classificationRegistry)
        {
            var tokens = GetTokens(buffer);
            var line = span.Start.GetContainingLine().LineNumber;

            foreach (var token in tokens.GetTokensForLine(line))
            {
                if (token.Classification != null)
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

            var context = tokens.GetContext(span.Start);
            if (context != null)
            {
                return context.Identifiers.Select(i => new Completion("$" + i.Name, "$" + i.Name, i.QuickInfo, imageSource, null));
            }

            return new Completion[0];
        }

        // Quick info
        public string GetQuickInfo(ITextBuffer buffer, SnapshotSpan span)
        {
            var tokens = GetTokens(buffer);
            var token = tokens.GetToken(span.Start);

            if (token != null && token.QuickInfo != null)
            {
                return token.QuickInfo;
            }

            return null;
        }
    }
}