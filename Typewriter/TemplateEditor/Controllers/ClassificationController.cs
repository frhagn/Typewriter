using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Typewriter.TemplateEditor.Controllers
{
    [Export(typeof(IClassifierProvider)), ContentType(Constants.ContentType)]
    internal class ClassificationControllerProvider : IClassifierProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry { get; set; }

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => new ClassificationController(buffer, ClassificationRegistry));
        }
    }

    internal class ClassificationController : IClassifier
    {
        private readonly ITextBuffer buffer;
        private readonly IClassificationTypeRegistryService classificationRegistry;

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        internal ClassificationController(ITextBuffer buffer, IClassificationTypeRegistryService classificationRegistry)
        {
            this.buffer = buffer;
            this.classificationRegistry = classificationRegistry;
            this.buffer.Changed += (sender, args) =>
            {
                var span = new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length);

                var tempEvent = ClassificationChanged;
                tempEvent?.Invoke(this, new ClassificationChangedEventArgs(span));
            };
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            return Editor.Instance.GetClassificationSpans(buffer, span, classificationRegistry).ToList();
        }
    }
}