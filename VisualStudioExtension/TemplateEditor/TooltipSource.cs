using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace Typewriter.TemplateEditor
{
    internal class TooltipSource : IQuickInfoSource
    {
        private readonly TooltipSourceProvider provider;
        private readonly ITextBuffer buffer;
        private readonly Dictionary<string, string> keywords = new Dictionary<string, string>();

        public TooltipSource(TooltipSourceProvider provider, ITextBuffer buffer)
        {
            this.provider = provider;
            this.buffer = buffer;

            keywords.Add("Name", "Returns the name of the current item.");
            keywords.Add("name", "Returns the a camelCase version of the name of the current item.");
            keywords.Add("Methods", "Methods[template][(separator)]\nLoops over all methods in the current scope and renders the template for each method.");
            keywords.Add("Properties", "Properties[template][(separator)]\nLoops over all properties in the current scope and renders the template for each property.");
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> qiContent, out ITrackingSpan applicableToSpan)
        {
            var triggerPoint = session.GetTriggerPoint(buffer.CurrentSnapshot);
            if (triggerPoint.HasValue == false)
            {
                applicableToSpan = null;
                return;
            }

            var snapshot = triggerPoint.Value.Snapshot;
            var navigator = provider.NavigatorService.GetTextStructureNavigator(buffer);
            var extent = navigator.GetExtentOfWord(triggerPoint.Value);

            if (extent.Span.Start > 1)
            {
                var trigger = buffer.CurrentSnapshot.GetText(extent.Span.Start - 2, 2);
                if (trigger != "${")
                {
                    applicableToSpan = null;
                    return;
                }
            }

            var keyword = extent.Span.GetText();

            if (keywords.ContainsKey(keyword))
            {
                applicableToSpan = snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
                qiContent.Add(keywords[keyword]);
                return;
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

    [Export(typeof(IQuickInfoSourceProvider)), ContentType(Constants.ContentType)]
    [Name("Tooltip Source Provider")]
    internal class TooltipSourceProvider : IQuickInfoSourceProvider
    {
        [Import] 
        internal ITextStructureNavigatorSelectorService NavigatorService;

        //[Import] 
        //internal ITextBufferFactoryService TextBufferFactoryService;

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new TooltipSource(this, textBuffer);
        }
    }
}
