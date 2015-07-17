using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Typewriter.TemplateEditor.Controllers
{
    [Export(typeof(ITaggerProvider))]
    [ContentType(Constants.ContentType), TagType(typeof(IOutliningRegionTag))]
    internal sealed class OutliningControllerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => new OutliningController(buffer) as ITagger<T>);
        }
    }

    internal class OutliningController : ITagger<IOutliningRegionTag>
    {
        private readonly ITextBuffer buffer;

        public OutliningController(ITextBuffer buffer)
        {
            this.buffer = buffer;
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection normalizedSnapshotSpans)
        {
            var spans = Editor.Instance.GetCodeBlocks(buffer).ToList();

            foreach (var tag in spans)
            {
                var temp = TagsChanged;
                temp?.Invoke(this, new SnapshotSpanEventArgs(tag));
            }

            return spans.Select(s => new TagSpan<IOutliningRegionTag>(s, new OutliningRegionTag(false, false, "...", s.GetText())));
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}
