using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using Typewriter.TemplateEditor.Lexing;
using VSCommand = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;

namespace Typewriter.TemplateEditor.Controllers
{
    internal class FormattingController : IOleCommandTarget
    {
        private readonly IOleCommandTarget nextCommandHandler;
        private readonly ITextView textView;

        public FormattingController(IVsTextView textViewAdapter, ITextView textView)
        {
            this.textView = textView;

            textViewAdapter.AddCommandFilter(this, out nextCommandHandler);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (pguidCmdGroup == VSConstants.VSStd2K)
            {
                switch ((VSCommand)prgCmds[0].cmdID)
                {
                    case VSCommand.FORMATDOCUMENT:
                    case VSCommand.FORMATSELECTION:
                        prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;
                        return VSConstants.S_OK;
                }
            }

            return nextCommandHandler.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (pguidCmdGroup == VSConstants.VSStd2K)
            {
                switch ((VSCommand)nCmdID)
                {
                    case VSCommand.FORMATDOCUMENT:
                        FormatSpan(0, textView.TextBuffer.CurrentSnapshot.Length);
                        break;
                    //case VSCommand.FORMATSELECTION:
                    //    FormatSelection();
                    //    break;
                }
            }

            return nextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        private void FormatSelection()
        {
            int start = textView.Selection.Start.Position.Position;
            int length = textView.Selection.End.Position.Position - start;

            FormatSpan(start, length);
        }

        private void FormatSpan(int start, int length)
        {
            Editor.Instance.FormatDocument(textView.TextBuffer);
        }
    }

    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("tst")]
    [Name("formatting handler")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal sealed class FormattingControllerProvider : IVsTextViewCreationListener
    {
        [Import]
        internal IVsEditorAdaptersFactoryService AdapterService { get; set; }

        //[Import]
        //internal ICompletionBroker CompletionBroker { get; set; }

        //[Import]
        //internal SVsServiceProvider ServiceProvider { get; set; }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            ITextView textView = AdapterService.GetWpfTextView(textViewAdapter);
            textView?.Properties.GetOrCreateSingletonProperty(() => new FormattingController(textViewAdapter, textView));
        }
    }
}
