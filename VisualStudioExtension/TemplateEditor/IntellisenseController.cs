using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Typewriter.TemplateEditor
{
    internal class IntellisenseController : IIntellisenseController
    {
        private ITextView view;
        private readonly IList<ITextBuffer> buffers;
        private readonly IntellisenseControllerProvider provider;

        internal IntellisenseController(ITextView view, IList<ITextBuffer> buffers, IntellisenseControllerProvider provider)
        {
            this.view = view;
            this.buffers = buffers;
            this.provider = provider;

            view.MouseHover += this.OnTextViewMouseHover;
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
            if (this.view != textView) return;

            textView.MouseHover -= this.OnTextViewMouseHover;
            this.view = null;
        }

        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }

        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }
    }

    [Export(typeof(IIntellisenseControllerProvider)), ContentType(Constants.ContentType)]
    [Name("Intellisense Controller Provider")]
    internal class IntellisenseControllerProvider : IIntellisenseControllerProvider
    {
        [Import] 
        internal IQuickInfoBroker QuickInfoBroker;

        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            return new IntellisenseController(textView, subjectBuffers, this);
        }
    }
}
