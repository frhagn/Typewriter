using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace Typewriter.TemplateEditor.Controllers
{
    [Export(typeof(IAsyncQuickInfoSourceProvider)), ContentType(Constants.ContentType)]
    [Name("Tooltip Source Provider")]
    internal class AsyncQuickInfoSourceProvider : IAsyncQuickInfoSourceProvider
    {
        [Import]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        public IAsyncQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            // This ensures only one instance per textbuffer is created
            return textBuffer.Properties.GetOrCreateSingletonProperty(() => new AsyncQuickInfoSource(this, textBuffer));
        }
    }

    internal class AsyncQuickInfoSource : IAsyncQuickInfoSource
    {
        private readonly AsyncQuickInfoSourceProvider provider;
        private readonly ITextBuffer buffer;

        public AsyncQuickInfoSource(AsyncQuickInfoSourceProvider provider, ITextBuffer buffer)
        {
            this.provider = provider;
            this.buffer = buffer;
        }

        public Task<QuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            var triggerPoint = session.GetTriggerPoint(buffer.CurrentSnapshot);
            if (triggerPoint.HasValue)
            {
                var extent = GetExtentOfWord(triggerPoint.Value);
                if (extent.HasValue)
                {
                    var line = triggerPoint.Value.GetContainingLine();
                    var lineNumber = triggerPoint.Value.GetContainingLine().LineNumber;
                    var lineSpan = buffer.CurrentSnapshot.CreateTrackingSpan(line.Extent, SpanTrackingMode.EdgeInclusive);
                    var lineNumberElm = new ContainerElement(
                        ContainerElementStyle.Wrapped,
                        new ClassifiedTextElement(
                            new ClassifiedTextRun(PredefinedClassificationTypeNames.Keyword, "Line number: "),
                            new ClassifiedTextRun(PredefinedClassificationTypeNames.Identifier, $"{lineNumber + 1}")
                        ));

                    var dateElm = new ContainerElement(
                        ContainerElementStyle.Stacked,
                        lineNumberElm,
                        extent.Value);

                    return Task.FromResult(new QuickInfoItem(lineSpan, dateElm));
                }
            }

            return Task.FromResult<QuickInfoItem>(null);
        }

        private bool disposed;

        public void Dispose()
        {
            if (disposed) return;

            GC.SuppressFinalize(this);
            disposed = true;
        }

        private static SnapshotSpan? GetExtentOfWord(SnapshotPoint point)
        {
            var line = point.GetContainingLine();

            if (line == null) return null;

            var text = line.GetText();
            var index = point - line.Start;

            var start = index;
            var length = 0;

            if (index > 0 && index < line.Length)
            {
                for (var i = index; i >= 0; i--)
                {
                    var current = text[i];
                    if (current == '$')
                    {
                        start = i;
                        length++;
                        break;
                    }

                    if (current != '_' && char.IsLetterOrDigit(current) == false) break;

                    start = i;
                    length++;
                }
            }

            if (length > 0)
            {
                index++;

                if (index < line.Length)
                {
                    for (var i = index; i < line.Length; i++)
                    {
                        var current = text[i];
                        if (current != '_' && char.IsLetterOrDigit(current) == false) break;

                        length++;
                    }
                }

                var span = new SnapshotSpan(point.Snapshot, start + line.Start, length);

                //Log.Debug("[" + span.GetText() + "]");

                return span;
            }

            return null;
        }
    }
}
