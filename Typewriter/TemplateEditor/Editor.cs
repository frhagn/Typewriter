using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Typewriter.TemplateEditor.Lexing;
using Typewriter.TemplateEditor.Lexing.Roslyn;
using Typewriter.VisualStudio;
using SemanticModel = Typewriter.TemplateEditor.Lexing.SemanticModel;

namespace Typewriter.TemplateEditor
{
    public class Editor
    {
        public static Editor Instance { get; } = new Editor();

        private readonly ShadowClass shadowClass = new ShadowClass();
        private readonly CodeLexer codeLexer = new CodeLexer();
        private readonly TemplateLexer templateLexer = new TemplateLexer();

        private ITextSnapshot currentSnapshot;
        private SemanticModel semanticModelCache;

        private Editor()
        {
        }

        private SemanticModel GetSemanticModel(ITextBuffer buffer)
        {
            if (currentSnapshot == buffer.CurrentSnapshot)
                return semanticModelCache;

            currentSnapshot = buffer.CurrentSnapshot;
            semanticModelCache = new SemanticModel(shadowClass);

            var code = currentSnapshot.GetText();

            codeLexer.Tokenize(semanticModelCache, code);
            templateLexer.Tokenize(semanticModelCache, code);
            
            return semanticModelCache;
        }

        // Outlining
        public IEnumerable<SnapshotSpan> GetCodeBlocks(ITextBuffer buffer)
        {
            var tokens = GetSemanticModel(buffer);
            var contextSpans = tokens.GetContextSpans(ContextType.CodeBlock);

            return contextSpans.Select(s => new SnapshotSpan(buffer.CurrentSnapshot, s.Start, s.End - s.Start));
        }

        // Brace matching
        public IEnumerable<ITagSpan<TextMarkerTag>> GetBraceTags(ITextBuffer buffer, SnapshotPoint point)
        {
            var tokens = GetSemanticModel(buffer);
            var token = tokens.GetToken(point.Position - 1);
            var tag = new TextMarkerTag(Classifications.BraceHighlight);

            if (token?.MatchingToken != null && token.IsOpen == false)
            {
                yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(point.Snapshot, token.Start, 1), tag);
                yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(point.Snapshot, token.MatchingToken.Start, 1), tag);
            }
            else
            {
                token = tokens.GetToken(point.Position);

                if (token?.MatchingToken != null && token.IsOpen)
                {
                    yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(point.Snapshot, token.Start, 1), tag);
                    yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(point.Snapshot, token.MatchingToken.Start, 1), tag);
                }
            }
        }

        // Classification
        public IEnumerable<ClassificationSpan> GetClassificationSpans(ITextBuffer buffer, SnapshotSpan span, IClassificationTypeRegistryService classificationRegistry)
        {
            var tokens = GetSemanticModel(buffer);

            foreach (var token in tokens.GetTokens(span.Span))
            {
                if (token.Classification != null)
                {
                    var classificationType = classificationRegistry.GetClassificationType(token.Classification);
                    yield return new ClassificationSpan(new SnapshotSpan(span.Snapshot, token.Start, token.Length), classificationType);
                }
            }
        }

        // Statement completion
        public bool EnableFullIntelliSense(ITextBuffer buffer, SnapshotPoint point)
        {
            var tokens = GetSemanticModel(buffer);
            var type = tokens.GetContextSpan(point).Type;
            
            return type == ContextType.CodeBlock || type == ContextType.Lambda;
        }

        public IEnumerable<Completion> GetCompletions(ITextBuffer buffer, SnapshotSpan span, IGlyphService glyphService)
        {
            var semanticModel = GetSemanticModel(buffer);
            var identifiers = semanticModel.GetIdentifiers(span.Start);
            var contextSpan = semanticModel.GetContextSpan(span.Start);
            var prefix = contextSpan.Type == ContextType.Template ? "$" : "";

            return identifiers.Select(i =>
            {
                var imageSource = glyphService.GetGlyph(i.Glyph, StandardGlyphItem.GlyphItemPublic);
                var quickInfo = i.IsParent ? i.QuickInfo.Replace("$parent", contextSpan.ParentContext?.Name.ToLowerInvariant()) : i.QuickInfo;

                return new Completion(prefix + i.Name, prefix + i.Name, quickInfo, imageSource, null);
            });
        }

        // Quick info
        public string GetQuickInfo(ITextBuffer buffer, SnapshotSpan span)
        {
            var semanticModel = GetSemanticModel(buffer);
            return semanticModel.GetQuickInfo(span.Start);
        }

        public void FormatDocument(ITextBuffer buffer)
        {
            //var SemanticModel = GetSemanticModel(buffer);
            //var section = SemanticModel.GetTokenSection(ContextType.CodeBlock);
            //var formatted = templateLexer.shadowClass.FormatDocument(section.Start, section.End);

            //var length = section.End - section.Start;

            //using (var edit = buffer.CreateEdit())
            //{
            //    edit.Replace(section.Start, length, formatted);
            //    edit.Apply();
            //}
        }
    }
}