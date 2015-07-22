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

            // ReSharper disable once UnusedVariable (used to suppress build warning)
            var temp = TagsChanged;
        }

        public IEnumerable<ITagSpan<ErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return spans.SelectMany(s => Editor.Instance.GetSyntaxErrorTags(buffer, s));
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}