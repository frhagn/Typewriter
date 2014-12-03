using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace Typewriter.TemplateEditor.Controllers
{
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("tst")]
    [Name("token completion1")]
    [Order(Before = "High")]
    internal class CompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        [Import]
        internal IGlyphService GlyphService { get; set; }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new CompletionSource(this, textBuffer, GlyphService);
        }
    }

    internal class CompletionSource : ICompletionSource
    {
        private readonly CompletionSourceProvider sourceProvider;
        private readonly ITextBuffer buffer;
        private readonly IGlyphService glyphService;

        public CompletionSource(CompletionSourceProvider sourceProvider, ITextBuffer buffer, IGlyphService glyphService)
        {
            this.sourceProvider = sourceProvider;
            this.buffer = buffer;
            this.glyphService = glyphService;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            var currentPoint = (session.TextView.Caret.Position.BufferPosition) - 1;
            var navigator = sourceProvider.NavigatorService.GetTextStructureNavigator(buffer);
            var extent = navigator.GetExtentOfWord(currentPoint);
            var token = currentPoint.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);

            var completions = Editor.Instance.GetCompletions(buffer, extent.Span, glyphService);

            completionSets.Add(new CompletionSet("Tokens", "Tokens", token, completions, null));
        }

        private bool disposed;

        public void Dispose()
        {
            if (!disposed)
            {
                GC.SuppressFinalize(this);
                disposed = true;
            }
        }
    }

    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("tst")]
    [Name("token completion handler")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal class CompletionControllerProvider : IVsTextViewCreationListener
    {
        [Import]
        internal IVsEditorAdaptersFactoryService AdapterService { get; set; }

        [Import]
        internal ICompletionBroker CompletionBroker { get; set; }

        [Import]
        internal SVsServiceProvider ServiceProvider { get; set; }

        //[Import]
        //internal ISignatureHelpBroker SignatureHelpBroker { get; set; }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            ITextView textView = AdapterService.GetWpfTextView(textViewAdapter);
            if (textView == null)
                return;

            textView.Properties.GetOrCreateSingletonProperty(() => new CompletionController(textViewAdapter, textView, this));//, SignatureHelpBroker));
        }
    }

    internal class CompletionController : IOleCommandTarget
    {
        private readonly IOleCommandTarget nextCommandHandler;
        private readonly ITextView textView;
        private readonly CompletionControllerProvider provider;
        //private readonly ISignatureHelpBroker broker;
        private ICompletionSession session;

        internal CompletionController(IVsTextView textViewAdapter, ITextView textView, CompletionControllerProvider provider)//, ISignatureHelpBroker broker)
        {
            this.textView = textView;
            this.provider = provider;
            //this.broker = broker;

            textViewAdapter.AddCommandFilter(this, out nextCommandHandler);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (VsShellUtilities.IsInAutomationFunction(provider.ServiceProvider))
            {
                return nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            }

            var commandId = nCmdID;
            var typedChar = char.MinValue;
            
            
            if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
            {
                typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
            }

            
            if (nCmdID == (uint)VSConstants.VSStd2KCmdID.RETURN || nCmdID == (uint)VSConstants.VSStd2KCmdID.TAB || (char.IsWhiteSpace(typedChar) || char.IsPunctuation(typedChar)))
            {
                
                if (session != null && !session.IsDismissed)
                {
                    if (session.SelectedCompletionSet.SelectionStatus.IsSelected)
                    {
                        session.Commit();

                        //broker.TriggerSignatureHelp(textView);

                        return VSConstants.S_OK;
                    }
                    
                    session.Dismiss();
                }
            }

            //pass along the command so the char is added to the buffer 
            var retVal = nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut); // Error här
            var handled = false;
            
            if (!typedChar.Equals(char.MinValue) && char.IsLetterOrDigit(typedChar))
            {
                if (session == null || session.IsDismissed) // If there is no active session, bring up completion
                {
                    if (this.TriggerCompletion())
                    {
                        if (session != null && session.IsDismissed == false)
                        {
                            session.Filter();
                        }
                    }
                }
                else
                {
                    session.Filter();
                }

                handled = true;
            }
            else if (commandId == (uint)VSConstants.VSStd2KCmdID.BACKSPACE || commandId == (uint)VSConstants.VSStd2KCmdID.DELETE)
            {
                if (session != null && !session.IsDismissed)
                {
                    session.Filter();
                }

                handled = true;
            }

            return handled ? VSConstants.S_OK : retVal;
        }

        private bool TriggerCompletion()
        {
            var caretPoint = textView.Caret.Position.Point.GetPoint(textBuffer => (!textBuffer.ContentType.IsOfType("projection")), PositionAffinity.Predecessor);
            if (!caretPoint.HasValue)
            {
                return false;
            }

            session = provider.CompletionBroker.CreateCompletionSession(textView, caretPoint.Value.Snapshot.CreateTrackingPoint(caretPoint.Value.Position, PointTrackingMode.Positive), true);

            session.Dismissed += this.OnSessionDismissed;
            session.Start();

            return true;
        }

        private void OnSessionDismissed(object sender, EventArgs e)
        {
            session.Dismissed -= this.OnSessionDismissed;
            session = null;
        }
    }
}