//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel.Composition;
//using System.Diagnostics;
//using System.Runtime.InteropServices;
//using Microsoft.VisualStudio;
//using Microsoft.VisualStudio.Editor;
//using Microsoft.VisualStudio.Language.Intellisense;
//using Microsoft.VisualStudio.OLE.Interop;
//using Microsoft.VisualStudio.Text;
//using Microsoft.VisualStudio.Text.Editor;
//using Microsoft.VisualStudio.Text.Operations;
//using Microsoft.VisualStudio.TextManager.Interop;
//using Microsoft.VisualStudio.Utilities;

//namespace Typewriter.TemplateEditor.Controllers
//{
//    internal class Parameter : IParameter
//    {
//        public Parameter(string name, string documentation, Span locus, ISignature signature)
//        {
//            Name = name;
//            Documentation = documentation;
//            Locus = locus;
//            Signature = signature;
//        }

//        public string Name { get; private set; }
//        public string Documentation { get; private set; }
//        public Span Locus { get; private set; }
//        public ISignature Signature { get; private set; }
//        public Span PrettyPrintedLocus { get; private set; }
//    }

//    internal class Signature : ISignature
//    {
//        private readonly ITextView textView;
//        private IParameter currentParameter;

//        internal Signature(ITextView textView, string content, string documentation, ReadOnlyCollection<IParameter> parameters)
//        {
//            Content = content;
//            Documentation = documentation;
//            Parameters = parameters;
//            this.textView = textView;
//        }
        
//        public IParameter CurrentParameter
//        {
//            get 
//            {
//                return currentParameter; 
//            }
//            set
//            {
//                if (currentParameter != value)
//                {
//                    var previousParameter = currentParameter;
//                    var tempHandler = this.CurrentParameterChanged;
                    
//                    currentParameter = value;

//                    if (tempHandler != null)
//                    {
//                        tempHandler(this, new CurrentParameterChangedEventArgs(previousParameter, currentParameter));
//                    }
//                }
//            }
//        }

//        internal void ComputeCurrentParameter()
//        {
//            if (Parameters.Count == 0)
//            {
//                this.CurrentParameter = null;
//                return;
//            }

//            //the number of commas in the string is the index of the current parameter 
//            var sigText = ApplicableToSpan.GetText(textView.TextBuffer.CurrentSnapshot);

//            Log.Print("{0}", textView.Caret.Position.BufferPosition.Position);

//            var currentIndex = 0;
//            var commaCount = 0;
//            while (currentIndex < sigText.Length)
//            {
//                var commaIndex = sigText.IndexOf(',', currentIndex);
//                if (commaIndex == -1)
//                {
//                    break;
//                }
//                commaCount++;
//                currentIndex = commaIndex + 1;
//            }

//            if (commaCount < Parameters.Count)
//            {
//                this.CurrentParameter = Parameters[commaCount];
//            }
//            else
//            {
//                //too many commas, so use the last parameter as the current one. 
//                this.CurrentParameter = Parameters[Parameters.Count - 1];
//            }
//        }

//        internal void OnSubjectBufferChanged(object sender, TextContentChangedEventArgs e)
//        {
//            this.ComputeCurrentParameter();
//        }

//        public ITrackingSpan ApplicableToSpan { get; set; }
//        public string Content { get; private set; }
//        public string PrettyPrintedContent { get; set; }
//        public string Documentation { get; private set; }
//        public ReadOnlyCollection<IParameter> Parameters { get; set; }
//        public event EventHandler<CurrentParameterChangedEventArgs> CurrentParameterChanged;
//    }

//    //

//    //Exec
//    //TryCreateSignatureHelpSource
//    //AugmentSignatureHelpSession
//    //CreateSignature
//    //CreateSignature
//    //CreateSignature


//    internal class SignatureHelpSource : ISignatureHelpSource
//    {
//        private readonly ITextBuffer buffer;

//        public SignatureHelpSource(ITextBuffer textBuffer)
//        {
//            buffer = textBuffer;
//        }

//        public void AugmentSignatureHelpSession(ISignatureHelpSession session, IList<ISignature> signatures)
//        {
//            var snapshot = buffer.CurrentSnapshot;
//            var position = session.GetTriggerPoint(buffer).GetPosition(snapshot);
//            var applicableToSpan = buffer.CurrentSnapshot.CreateTrackingSpan(new Span(position, 0), SpanTrackingMode.EdgeInclusive, 0);

//            signatures.Add(CreateSignature(session.TextView, "Properties[template]", "Documentation for string.", applicableToSpan));
//            signatures.Add(CreateSignature(session.TextView, "Properties[template, separator]", "Documentation for adding doubles.", applicableToSpan));
//            signatures.Add(CreateSignature(session.TextView, "Properties[index]", "Documentation for adding integers.", applicableToSpan));
//        }

//        public ISignature GetBestMatch(ISignatureHelpSession session)
//        {
//            //if (session.Signatures.Count > 0)
//            //{
//            //    ITrackingSpan applicableToSpan = session.Signatures[0].ApplicableToSpan;
//            //    string text = applicableToSpan.GetText(applicableToSpan.TextBuffer.CurrentSnapshot);

//            //    if (text.Trim().Equals("Properties"))  //get only "add"  
//            //        return session.Signatures[0];
//            //}
//            return null;
//        }

//        private Signature CreateSignature(ITextView textView, string methodSig, string methodDoc, ITrackingSpan span)
//        {
//            Debug.Print("CreateSignature");
//            var sig = new Signature(textView, methodSig, methodDoc, null);
//            textView.TextBuffer.Changed += sig.OnSubjectBufferChanged;

//            //find the parameters in the method signature (expect methodname(one, two) 
//            var pars = methodSig.Split(new char[] { '[', ',', ']' });
//            var paramList = new List<IParameter>();

//            var locusSearchStart = 0;
//            for (var i = 1; i < pars.Length; i++)
//            {
//                var param = pars[i].Trim();

//                if (string.IsNullOrEmpty(param))
//                    continue;

//                //find where this parameter is located in the method signature 
//                var locusStart = methodSig.IndexOf(param, locusSearchStart);
//                if (locusStart >= 0)
//                {
//                    var locus = new Span(locusStart, param.Length);
//                    locusSearchStart = locusStart + param.Length;
//                    paramList.Add(new Parameter(param, "Documentation for the parameter.", locus, sig));
//                }
//            }

//            sig.Parameters = new ReadOnlyCollection<IParameter>(paramList);
//            sig.ApplicableToSpan = span;
//            sig.ComputeCurrentParameter();

//            return sig;
//        }

//        private bool disposed;

//        public void Dispose()
//        {
//            if (disposed == false)
//            {
//                GC.SuppressFinalize(this);
//                disposed = true;
//            }
//        }
//    }

//    [Export(typeof(ISignatureHelpSourceProvider))]
//    [Name("Signature Help source")]
//    [Order(Before = "High")]
//    [ContentType(Constants.ContentType)]
//    internal class SignatureHelpSourceProvider : ISignatureHelpSourceProvider
//    {
//        public ISignatureHelpSource TryCreateSignatureHelpSource(ITextBuffer textBuffer)
//        {
//            return new SignatureHelpSource(textBuffer);
//        }
//    }

//    internal sealed class SignatureHelpController : IOleCommandTarget
//    {
//        private readonly IOleCommandTarget nextCommandHandler;
//        private readonly ITextView textView;
//        private readonly ISignatureHelpBroker broker;
//        private readonly ITextStructureNavigator navigator;
//        private ISignatureHelpSession session;

//        internal SignatureHelpController(IVsTextView textViewAdapter, ITextView textView, ITextStructureNavigator nav, ISignatureHelpBroker broker)
//        {
//            this.textView = textView;
//            this.broker = broker;
//            this.navigator = nav;

//            textViewAdapter.AddCommandFilter(this, out nextCommandHandler);
//        }

//        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
//        {
//            if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
//            {
//                var typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
//                if (typedChar.Equals('['))
//                {
//                    //move the point back so it's in the preceding word
//                    var point = textView.Caret.Position.BufferPosition - 1;
//                    var extent = navigator.GetExtentOfWord(point);
//                    var info = Editor.Instance.GetQuickInfo(textView.TextBuffer, extent.Span);
//                    if (info != null)
//                    {
//                        session = broker.TriggerSignatureHelp(textView);
//                    }
//                }
//                else if (typedChar.Equals(']') && session != null)
//                {
//                    session.Dismiss();
//                    session = null;
//                }
//            }

//            return nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
//        }

//        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
//        {
//            return nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
//        }
//    }

//    [Export(typeof(IVsTextViewCreationListener))]
//    [Name("Signature Help controller")]
//    [TextViewRole(PredefinedTextViewRoles.Editable)]
//    [ContentType(Constants.ContentType)]
//    internal class SignatureHelpControllerProvider : IVsTextViewCreationListener
//    {
//        [Import]
//        internal IVsEditorAdaptersFactoryService AdapterService { get; set; }

//        [Import]
//        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

//        [Import]
//        internal ISignatureHelpBroker SignatureHelpBroker { get; set; }

//        public void VsTextViewCreated(IVsTextView textViewAdapter)
//        {
//            ITextView textView = AdapterService.GetWpfTextView(textViewAdapter);
//            if (textView == null)
//                return;

//            textView.Properties.GetOrCreateSingletonProperty(() => new SignatureHelpController(textViewAdapter, textView, NavigatorService.GetTextStructureNavigator(textView.TextBuffer), SignatureHelpBroker));
//        }
//    }
//}
