using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Typewriter.TemplateEditor
{
    internal class Classifier : IClassifier
    {
        private readonly ITextBuffer buffer;
        private readonly IClassificationTypeRegistryService classificationRegistry;

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        internal Classifier(ITextBuffer buffer, IClassificationTypeRegistryService classificationRegistry)
        {
            this.buffer = buffer;
            this.classificationRegistry = classificationRegistry;
            this.buffer.Changed += (sender, args) =>
            {
                var span = new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length);
                ClassificationChanged(this, new ClassificationChangedEventArgs(span));
            };
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var list = new List<ClassificationSpan>();

            var code = buffer.CurrentSnapshot.GetText(span);

            foreach (var keyword in keywords.Keys)
            {
                var length = keyword.Length;

                for (var i = 0; ; i += length)
                {
                    i = code.IndexOf(keyword, i, StringComparison.Ordinal);

                    if (i == -1) break;

                    var start = i + span.Start.Position;

                    if (keyword == "//")
                    {
                        var end = span.Snapshot.GetLineFromPosition(start).End.Position - start;
                        var c = classificationRegistry.GetClassificationType(Constants.CommentClassificationType);

                        list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, start, end), c));
                        
                        break;
                    }

                    if (i > 0 && char.IsLetterOrDigit(code, i - 1)) continue;
                    if (i + length < code.Length && char.IsLetterOrDigit(code, i + length)) continue;

                    var type = keywords[keyword];

                    if (type == Constants.PropertyClassificationType)
                    {
                        if (i < 2) continue;
                        if (code.Substring(i - 2, 2) != "${") continue;
                    }

                    var classificationType = classificationRegistry.GetClassificationType(type);
                    list.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, start, length), classificationType));
                }
            }

            return list;
        }

        private readonly Dictionary<string, string> keywords = new Dictionary<string, string>
        {
            {"//",Constants.CommentClassificationType},

            {"name",Constants.PropertyClassificationType},
            {"Name",Constants.PropertyClassificationType},
            {"Methods",Constants.PropertyClassificationType},
            {"Properties",Constants.PropertyClassificationType},
            {"Classes",Constants.PropertyClassificationType},
            {"TypeName",Constants.PropertyClassificationType},

            { "module", Constants.KeywordClassificationType },
            { "class", Constants.KeywordClassificationType },
            { "export", Constants.KeywordClassificationType },
            { "number", Constants.KeywordClassificationType },
            { "string", Constants.KeywordClassificationType },
            { "constructor", Constants.KeywordClassificationType },
            { "public", Constants.KeywordClassificationType },
            { "private", Constants.KeywordClassificationType },
            { "protected", Constants.KeywordClassificationType },
            { "return", Constants.KeywordClassificationType },
            { "void", Constants.KeywordClassificationType },
            { "null", Constants.KeywordClassificationType },
            { "boolean", Constants.KeywordClassificationType },
            { "any", Constants.KeywordClassificationType }
        };
    }

    [Export(typeof(IClassifierProvider)), ContentType(Constants.ContentType)]
    internal class TstClassifierProvider : IClassifierProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry;

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => new Classifier(buffer, ClassificationRegistry));
        }
    }
}
