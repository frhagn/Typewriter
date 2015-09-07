using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using Typewriter.VisualStudio;

namespace Typewriter.TemplateEditor.Controllers
{
    [Export(typeof(IQuickInfoSourceProvider)), ContentType(Constants.ContentType)]
    [Name("Tooltip Source Provider")]
    internal class QuickInfoSourceProvider : IQuickInfoSourceProvider
    {
        [Import]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new QuickInfoSource(this, textBuffer);
        }
    }

    internal class QuickInfoSource : IQuickInfoSource
    {
        private readonly QuickInfoSourceProvider provider;
        private readonly ITextBuffer buffer;

        public QuickInfoSource(QuickInfoSourceProvider provider, ITextBuffer buffer)
        {
            this.provider = provider;
            this.buffer = buffer;
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
        {
            var snapshot = buffer.CurrentSnapshot;
            var triggerPoint = session.GetTriggerPoint(snapshot);

            if (triggerPoint.HasValue)
            {
                var extent = GetExtentOfWord(triggerPoint.Value);
                if (extent.HasValue)
                {
                    var info = Editor.Instance.GetQuickInfo(buffer, extent.Value);

                    if (info != null)
                    {
                        applicableToSpan = snapshot.CreateTrackingSpan(extent.Value, SpanTrackingMode.EdgeInclusive);
                        quickInfoContent.Add(info);
                        return;
                    }
                }
            }

            applicableToSpan = null;
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

    [Export(typeof(IIntellisenseControllerProvider)), ContentType(Constants.ContentType)]
    [Name("Intellisense Controller Provider")]
    internal class QuickInfoControllerProvider : IIntellisenseControllerProvider
    {
        [Import]
        internal IQuickInfoBroker QuickInfoBroker { get; set; }

        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            return new QuickInfoController(textView, subjectBuffers, this);
        }
    }

    internal class QuickInfoController : IIntellisenseController
    {
        private ITextView view;
        private readonly IList<ITextBuffer> buffers;
        private readonly QuickInfoControllerProvider provider;

        internal QuickInfoController(ITextView view, IList<ITextBuffer> buffers, QuickInfoControllerProvider provider)
        {
            this.view = view;
            this.buffers = buffers;
            this.provider = provider;

            view.MouseHover += OnTextViewMouseHover;
        }

        private void OnTextViewMouseHover(object sender, MouseHoverEventArgs e)
        {
            var point = view.BufferGraph.MapDownToFirstMatch(new SnapshotPoint(view.TextSnapshot, e.Position), PointTrackingMode.Positive,
                snapshot => buffers.Contains(snapshot.TextBuffer), PositionAffinity.Predecessor);

            if (point == null) return;

            if (!provider.QuickInfoBroker.IsQuickInfoActive(view))
            {
                var triggerPoint = point.Value.Snapshot.CreateTrackingPoint(point.Value.Position, PointTrackingMode.Positive);
                provider.QuickInfoBroker.TriggerQuickInfo(view, triggerPoint, true);
            }
        }

        public void Detach(ITextView textView)
        {
            if (view != textView) return;

            textView.MouseHover -= OnTextViewMouseHover;
            view = null;
        }

        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }

        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }
    }
}
