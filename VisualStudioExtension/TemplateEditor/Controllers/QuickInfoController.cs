using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace Typewriter.TemplateEditor.Controllers
{
    [Export(typeof(IQuickInfoSourceProvider)), ContentType(Constants.ContentType)]
    [Name("Tooltip Source Provider")]
    internal class QuickInfoSourceProvider : IQuickInfoSourceProvider
    {
        [Import]
        internal ITextStructureNavigatorSelectorService NavigatorService;
        

        //[ImportMany]
        //IEnumerable<EditorFormatDefinition> EditorFormats { get; set; }

        //static void PrintNameAttribute(Type t, string displayName)
        //{
        //    NameAttribute MyAttribute = (NameAttribute)Attribute.GetCustomAttribute(t, typeof(NameAttribute));
        //    if (MyAttribute == null)
        //    {
        //        Log.Print(String.Format("'{0}' = null", displayName));
        //    }
        //    else
        //    {
        //        Log.Print(String.Format("'{0}' = '{1}'", displayName, MyAttribute.Name));
        //    }
        //}

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            //Log.Print("Predefined EditorFormatDefinition");
            //foreach (EditorFormatDefinition element in EditorFormats)
            //{
            //    PrintNameAttribute(element.GetType(), element.DisplayName);
            //}
            //Log.Print("Done!");
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
                var navigator = provider.NavigatorService.GetTextStructureNavigator(buffer);
                var extent = navigator.GetExtentOfWord(triggerPoint.Value);
                var info = Editor.Instance.GetQuickInfo(buffer, extent.Span);

                if (info != null)
                {
                    applicableToSpan = snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
                    quickInfoContent.Add(info);
                    return;
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
    }

    [Export(typeof(IIntellisenseControllerProvider)), ContentType(Constants.ContentType)]
    [Name("Intellisense Controller Provider")]
    internal class QuickInfoControllerProvider : IIntellisenseControllerProvider
    {
        [Import]
        internal IQuickInfoBroker QuickInfoBroker;

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
}
