using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Typewriter.TemplateEditor.Controllers
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("tst")]
    [TagType(typeof(TextMarkerTag))]
    internal class BraceMatchingControllerProvider : IViewTaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView == null || textView.TextBuffer != buffer)
                return null;

            return new BraceMatchingController(textView, buffer) as ITagger<T>;
        }
    }

    internal class BraceMatchingController : ITagger<TextMarkerTag>
    {
        ITextView View { get; set; }
        ITextBuffer SourceBuffer { get; set; }
        SnapshotPoint? CurrentChar { get; set; }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        internal BraceMatchingController(ITextView view, ITextBuffer sourceBuffer)
        {
            this.View = view;
            this.SourceBuffer = sourceBuffer;
            this.CurrentChar = null;

            this.View.Caret.PositionChanged += CaretPositionChanged;
            this.View.LayoutChanged += ViewLayoutChanged;
        }


        void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (e.NewSnapshot != e.OldSnapshot)
            {
                UpdateAtCaretPosition(View.Caret.Position);
            }
        }

        void CaretPositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            UpdateAtCaretPosition(e.NewPosition);
        }

        void UpdateAtCaretPosition(CaretPosition caretPosition)
        {
            CurrentChar = caretPosition.Point.GetPoint(SourceBuffer, caretPosition.Affinity);

            if (CurrentChar.HasValue == false) return;

            var tempEvent = TagsChanged;
            if (tempEvent != null)
            {
                tempEvent(this, new SnapshotSpanEventArgs(new SnapshotSpan(SourceBuffer.CurrentSnapshot, 0, SourceBuffer.CurrentSnapshot.Length)));
            }
        }

        public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0) yield break;
            if (!CurrentChar.HasValue || CurrentChar.Value.Position > CurrentChar.Value.Snapshot.Length) yield break;

            var currentChar = CurrentChar.Value;

            if (spans[0].Snapshot != currentChar.Snapshot)
            {
                currentChar = currentChar.TranslateTo(spans[0].Snapshot, PointTrackingMode.Positive);

                if (currentChar.Position > currentChar.Snapshot.Length)
                    yield break;
            }

            foreach (var tag in Editor.Instance.GetBraceTags(SourceBuffer, new SnapshotSpan(currentChar, 1)))
            {
                yield return tag;
            }
        }
    }
}