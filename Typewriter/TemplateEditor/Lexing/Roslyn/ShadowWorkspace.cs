using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Recommendations;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CSharp.RuntimeBinder;
using Typewriter.CodeModel;
using Typewriter.VisualStudio;
using File = System.IO.File;

namespace Typewriter.TemplateEditor.Lexing.Roslyn
{
    internal class ShadowWorkspace : Workspace
    {
        private static int counter;
        private static readonly Assembly[] defaultReferences =
        {
            typeof (int).Assembly, // mscorelib
            typeof (Uri).Assembly, // System
            typeof (Enumerable).Assembly, // System.Core
            //typeof (XmlReader).Assembly, // System.Xml
            //typeof (XDocument).Assembly, // System.Xml.Linq
            typeof (RuntimeBinderException).Assembly, // Microsoft.CSharp
            typeof (Class).Assembly // Typewriter.CodeModel
        };

        private readonly ConcurrentDictionary<string, DocumentationProvider> documentationProviders;

        public ShadowWorkspace() : base(MefHostServices.DefaultHost, WorkspaceKind.Host)
        {
            documentationProviders = new ConcurrentDictionary<string, DocumentationProvider>();
        }

        public DocumentId AddProjectWithDocument(string documentFileName, string text)
        {
            var fileName = Path.GetFileName(documentFileName);
            var name = Path.GetFileNameWithoutExtension(documentFileName) + counter++;

            var projectId = ProjectId.CreateNewId();
            var references = defaultReferences.Select(CreateReference).ToList();
            var projectInfo = ProjectInfo.Create(projectId, new VersionStamp(), name, name + ".dll", LanguageNames.CSharp, metadataReferences: references, compilationOptions: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            OnProjectAdded(projectInfo);

            var documentId = DocumentId.CreateNewId(projectId);
            var documentInfo = DocumentInfo.Create(documentId, fileName, loader: TextLoader.From(TextAndVersion.Create(SourceText.From(text, Encoding.UTF8), VersionStamp.Create())));

            OnDocumentAdded(documentInfo);

            return documentId;
        }

        public void UpdateText(DocumentId documentId, string text)
        {
            OnDocumentTextChanged(documentId, SourceText.From(text, Encoding.UTF8), PreservationMode.PreserveValue);
        }

        public string FormatDocument(DocumentId documentId)
        {
            var document = CurrentSolution.GetDocument(documentId);
            document = Formatter.FormatAsync(document).Result;
            var text = document.GetTextAsync().Result;

            return text.ToString();
        }

        public IEnumerable<ClassifiedSpan> GetClassifiedSpans(DocumentId documentId, int start, int length)
        {
            var document = CurrentSolution.GetDocument(documentId);
            return Classifier.GetClassifiedSpansAsync(document, TextSpan.FromBounds(start, start + length)).Result;
        }

        public IReadOnlyList<ISymbol> GetRecommendedSymbols(DocumentId documentId, int position)
        {
            var document = CurrentSolution.GetDocument(documentId);
            var semanticModel = document.GetSemanticModelAsync().Result;

            return Recommender.GetRecommendedSymbolsAtPosition(semanticModel, position, this).ToArray();
        }

        public IReadOnlyList<MethodDeclarationSyntax> GetMethods(DocumentId documentId)
        {
            var document = CurrentSolution.GetDocument(documentId);
            var semanticModel = document.GetSemanticModelAsync().Result;

            var root = semanticModel.SyntaxTree.GetRoot();
            return root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToArray();
        }

        public void ChangeAllMethodsToPublicStatic(DocumentId documentId)
        {
            var document = CurrentSolution.GetDocument(documentId);
            var semanticModel = document.GetSemanticModelAsync().Result;

            var root = semanticModel.SyntaxTree.GetRoot();
            var count = root.DescendantNodes().OfType<MethodDeclarationSyntax>().Count();

            for (var i = 0; i < count; i++)
            {
                var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToArray()[i];
                var trivia = method.ReturnType.GetTrailingTrivia();
                var modifiers = SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword).WithTrailingTrivia(trivia), SyntaxFactory.Token(SyntaxKind.StaticKeyword).WithTrailingTrivia(trivia));
                root = root.ReplaceNode(method, method.WithModifiers(modifiers));
            }

            document = document.WithSyntaxRoot(root);
            var text = document.GetTextAsync().Result;

            UpdateText(documentId, text.ToString());
        }

        public EmitResult Compile(DocumentId documentId, string path)
        {
            var document = CurrentSolution.GetDocument(documentId);
            var compilation = document.Project.GetCompilationAsync().Result;

            using (var fileStream = File.Create(path))
            {
                return compilation.Emit(fileStream);
            }
        }

        public ISymbol GetSymbol(DocumentId documentId, int position)
        {
            var document = CurrentSolution.GetDocument(documentId);
            var semanticModel = document.GetSemanticModelAsync().Result;
            var root = semanticModel.SyntaxTree.GetRoot();
            var symbol = root.FindToken(position);

            return semanticModel.GetSymbolInfo(symbol.Parent).Symbol;
        }

        private MetadataReference CreateReference(Assembly assembly)
        {
            var location = assembly.Location;
            var path = Path.ChangeExtension(location, "xml");
            var provider = documentationProviders.GetOrAdd(path, p => new XmlDocumentationProvider(p));

            return MetadataReference.CreateFromFile(location, new MetadataReferenceProperties(), provider);
        }
    }
}
