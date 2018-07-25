using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
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
using Typewriter.VisualStudio;
using VSCommand = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;

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
            var point = session.TextView.Caret.Position.BufferPosition;
            var line = point.GetContainingLine();

            if (line == null) return;

            var text = line.GetText();
            var index = point - line.Start;

            var start = index;

            if (index > 0)
            {
                for (var i = index; i > 0; i--)
                {
                    var current = text[i - 1];
                    if (current == '$' || current == '#')
                    {
                        start = i - 1;
                        break;
                    }

                    if (current != '_' && char.IsLetterOrDigit(current) == false) break;

                    start = i - 1;
                }
            }

            var span = new SnapshotSpan(point.Snapshot, start + line.Start, point - (start + line.Start));
            //Log.Debug("[" + span.GetText() + "]");

            var trackingSpan = buffer.CurrentSnapshot.CreateTrackingSpan(span.Start, span.Length, SpanTrackingMode.EdgeInclusive);
            var completions = Editor.Instance.GetCompletions(buffer, span, glyphService);

            completionSets.Add(new StringCompletionSet("Identifiers", trackingSpan, completions));
        }

        class StringCompletionSet : CompletionSet
        {
            public StringCompletionSet(string moniker, ITrackingSpan span, IEnumerable<Completion> completions) : base(moniker, "Typewriter", span, completions, null) { }

            public override void SelectBestMatch()
            {
                base.SelectBestMatch(CompletionMatchType.MatchInsertionText, true);
                if (SelectionStatus.IsSelected) return;
                base.SelectBestMatch(CompletionMatchType.MatchInsertionText, false);
            }
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
        private ICompletionSession _currentSession;

        internal CompletionController(IVsTextView textViewAdapter, ITextView textView, CompletionControllerProvider provider)//, ISignatureHelpBroker broker)
        {
            this.textView = textView;
            this.provider = provider;
            //this.broker = broker;

            textViewAdapter.AddCommandFilter(this, out nextCommandHandler);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (pguidCmdGroup == VSConstants.VSStd2K)
            {
                switch ((VSCommand)prgCmds[0].cmdID)
                {
                    case VSCommand.AUTOCOMPLETE:
                    case VSCommand.COMPLETEWORD:
                    case VSCommand.SHOWMEMBERLIST:
                        prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;
                        return VSConstants.S_OK;
                }
            }

            return nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        //public int Exec(ref Guid pguidCmdGroup, uint commandId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        //{
        //    if (VsShellUtilities.IsInAutomationFunction(provider.ServiceProvider) || pguidCmdGroup != VSConstants.VSStd2K)
        //        return nextCommandHandler.Exec(ref pguidCmdGroup, commandId, nCmdexecopt, pvaIn, pvaOut);

        //    var command = (VSCommand)commandId;
        //    var typedChar = char.MinValue;

        //    if (command == VSCommand.TYPECHAR)
        //        typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);

        //    if (command == VSCommand.RETURN || command == VSCommand.TAB || (char.IsWhiteSpace(typedChar) || char.IsPunctuation(typedChar)))
        //    {
        //        if (session != null && !session.IsDismissed)
        //        {
        //            if (session.SelectedCompletionSet.SelectionStatus.IsSelected)
        //            {
        //                session.Commit();

        //                //broker.TriggerSignatureHelp(textView);

        //                if (command == VSCommand.RETURN || command == VSCommand.TAB)
        //                    return VSConstants.S_OK;
        //            }
        //            else
        //            {
        //                session.Dismiss();
        //            }
        //        }
        //    }

        //    //pass along the command so the char is added to the buffer 
        //    var retVal = nextCommandHandler.Exec(ref pguidCmdGroup, commandId, nCmdexecopt, pvaIn, pvaOut); // Error här
        //    var handled = false;

        //    if (command == VSCommand.AUTOCOMPLETE || command == VSCommand.COMPLETEWORD || char.IsLetterOrDigit(typedChar) || typedChar == '$' || typedChar == '.') // !typedChar.Equals(char.MinValue) && 
        //    {
        //        if (session == null || session.IsDismissed) // If there is no active session, bring up completion
        //        {
        //            if (this.TriggerCompletion())
        //            {
        //                if (session != null && session.IsDismissed == false && char.IsLetterOrDigit(typedChar))
        //                {
        //                    session.Filter();
        //                }
        //            }
        //        }
        //        else
        //        {
        //            session.Filter();
        //        }

        //        handled = true;
        //    }
        //    else if (command == VSCommand.BACKSPACE || command == VSCommand.DELETE)
        //    {
        //        if (session != null && !session.IsDismissed)
        //        {
        //            session.Filter();
        //        }

        //        handled = true;
        //    }

        //    return handled ? VSConstants.S_OK : retVal;
        //}

        //private bool TriggerCompletion()
        //{
        //    var caretPoint = textView.Caret.Position.Point.GetPoint(textBuffer => (!textBuffer.ContentType.IsOfType("projection")), PositionAffinity.Predecessor);
        //    if (!caretPoint.HasValue)
        //    {
        //        return false;
        //    }

        //    session = provider.CompletionBroker.CreateCompletionSession(textView, caretPoint.Value.Snapshot.CreateTrackingPoint(caretPoint.Value.Position, PointTrackingMode.Positive), true);

        //    session.Dismissed += this.OnSessionDismissed;
        //    session.Start();

        //    return true;
        //}

        //private void OnSessionDismissed(object sender, EventArgs e)
        //{
        //    session.Dismissed -= this.OnSessionDismissed;
        //    session = null;
        //}
        private static char GetTypeChar(IntPtr pvaIn)
        {
            return (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            bool handled = false;
            int hresult = VSConstants.S_OK;

            // 1. Pre-process
            if (pguidCmdGroup == VSConstants.VSStd2K)
            {
                switch ((VSCommand)nCmdID)
                {
                    case VSCommand.AUTOCOMPLETE:
                    case VSCommand.COMPLETEWORD:
                    case VSCommand.SHOWMEMBERLIST:
                        handled = StartSession();
                        break;
                    case VSCommand.RETURN:
                        handled = Complete(false);
                        break;
                    case VSCommand.TAB:
                        handled = Complete(true);
                        break;
                    case VSCommand.CANCEL:
                        handled = Cancel();
                        break;
                    case VSCommand.TYPECHAR:
                        char ch = GetTypeChar(pvaIn);
                        if (ch == '.' || ch == '[' || ch == '(' || ch == '$' || ch == '#' || ch == ' ' || ch == ';' || ch == '<')
                            Complete(false);
                        else if (ch == '"')
                            Cancel();
                            break;
                }
            }

            if (!handled)
                hresult = nextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

            if (ErrorHandler.Succeeded(hresult))
            {
                if (pguidCmdGroup == VSConstants.VSStd2K && (VSCommand)nCmdID == VSCommand.TYPECHAR)
                {
                    if (_currentSession != null)
                    {
                        Filter();
                    }
                    else if (textView != null)
                    {
                        var ch = GetTypeChar(pvaIn);

                        if (Editor.Instance.EnableFullIntelliSense(textView.TextBuffer, textView.Caret.Position.BufferPosition))
                        {
                            if (ch == '.')
                                StartSession();
                            else if (char.IsPunctuation(ch) == false && char.IsControl(ch) == false)
                                StartSession();
                        }
                        else if (ch == '$' || ch == '#')
                        {
                            StartSession();
                        }
                    }
                }
            }

            return hresult;
        }

        private void Filter()
        {
            if (_currentSession?.SelectedCompletionSet == null)
                return;

            _currentSession.SelectedCompletionSet.SelectBestMatch();
            _currentSession.SelectedCompletionSet.Recalculate();
        }

        bool Cancel()
        {
            if (_currentSession == null)
                return false;

            _currentSession.Dismiss();

            return true;
        }

        bool Complete(bool force)
        {
            if (_currentSession == null)
                return false;

            if (!_currentSession.SelectedCompletionSet.SelectionStatus.IsSelected && !force)
            {
                _currentSession.Dismiss();
                return false;
            }

            _currentSession.Commit();
            return true;
        }

        bool StartSession()
        {
            if (_currentSession != null)
                return false;

            var caret = textView.Caret.Position.BufferPosition;

            //if (caret.Position == 0)
            //    return false;

            var snapshot = caret.Snapshot;

            _currentSession = provider.CompletionBroker.IsCompletionActive(textView) ?
                provider.CompletionBroker.GetSessions(textView)[0] :
                provider.CompletionBroker.CreateCompletionSession(textView, snapshot.CreateTrackingPoint(caret, PointTrackingMode.Positive), true);

            _currentSession.Dismissed += (sender, args) => _currentSession = null;

            if (!_currentSession.IsStarted)
                _currentSession.Start();

            return true;
        }
    }
}