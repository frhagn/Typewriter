using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Typewriter.TemplateEditor.Controllers
{
    [Export(typeof (IViewTaggerProvider))]
    [ContentType(Constants.ContentType), TagType(typeof (TextMarkerTag))]
    internal class BraceMatchingControllerProvider : IViewTaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView == null || textView.TextBuffer != buffer)
                return null;

            return buffer.Properties.GetOrCreateSingletonProperty(() => new BraceMatchingController(textView, buffer) as ITagger<T>);
        }
    }

    internal class BraceMatchingController : ITagger<TextMarkerTag>
    {
        private readonly ITextView view;
        private readonly ITextBuffer buffer;
        private SnapshotPoint? snapshotPoint;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        internal BraceMatchingController(ITextView view, ITextBuffer buffer)
        {
            this.view = view;
            this.buffer = buffer;
            this.view.Caret.PositionChanged += CaretPositionChanged;
            this.view.LayoutChanged += ViewLayoutChanged;
        }

        private void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (e.NewSnapshot != e.OldSnapshot)
                UpdateAtCaretPosition(view.Caret.Position);
        }

        private void CaretPositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            UpdateAtCaretPosition(e.NewPosition);
        }

        private void UpdateAtCaretPosition(CaretPosition caretPosition)
        {
            snapshotPoint = caretPosition.Point.GetPoint(buffer, caretPosition.Affinity);

            if (snapshotPoint.HasValue == false) return;

            var tempEvent = TagsChanged;
            tempEvent?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length)));
        }

        public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0) yield break;
            if (snapshotPoint.HasValue == false) yield break;

            var point = snapshotPoint.Value;
            var snapshot = point.Snapshot;

            if (spans[0].Snapshot != snapshot)
                point = point.TranslateTo(spans[0].Snapshot, PointTrackingMode.Positive);

            if (point.Position > snapshot.Length)
                yield break;

            foreach (var tag in Editor.Instance.GetBraceTags(buffer, point)) //new SnapshotSpan(point, 1)))
            {
                yield return tag;
            }
        }
    }
}