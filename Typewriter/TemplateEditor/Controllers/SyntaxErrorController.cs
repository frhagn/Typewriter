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
    [ContentType(Constants.ContentType), TagType(typeof(ErrorTag))]
    internal class SyntaxErrorControllerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => new SyntaxErrorController(buffer) as ITagger<T>);
        }
    }

    internal class SyntaxErrorController : ITagger<ErrorTag>
    {
        private readonly ITextBuffer buffer;

        internal SyntaxErrorController(ITextBuffer buffer)
        {
            this.buffer = buffer;
            this.buffer.PostChanged += BufferOnPostChanged;
        }

        private void BufferOnPostChanged(object sender, EventArgs eventArgs)
        {
            var tagsChanged = TagsChanged;

            if (tagsChanged != null)
            {
                var span = new Span(0, buffer.CurrentSnapshot.Length);
                var snapshotSpan = new SnapshotSpan(buffer.CurrentSnapshot, span);
                tagsChanged(this, new SnapshotSpanEventArgs(snapshotSpan));
            }
        }

        public IEnumerable<ITagSpan<ErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return spans.SelectMany(s => Editor.Instance.GetSyntaxErrorTags(buffer, s));
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}