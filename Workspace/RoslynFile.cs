using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Typewriter.CodeModel.Collections;

namespace Typewriter.CodeModel.Roslyn
{
    public class RoslynFile : File
    {
        private readonly Document document;
        private readonly SyntaxNode root;

        public RoslynFile(Document document)
        {
            this.document = document;
            var semanticModel = document.GetSemanticModelAsync().Result;
            root = semanticModel.SyntaxTree.GetRoot();
        }

        public string Name => document.Name;
        public string FullName => document.FilePath;

        private ClassCollection classes;
        public ClassCollection Classes => classes ?? (classes = new ClassCollectionImpl(RoslynClass.FromClassSyntax(root.ChildNodes().OfType<NamespaceDeclarationSyntax>().SelectMany(n => n.ChildNodes().OfType<ClassDeclarationSyntax>()), this)));

        public EnumCollection Enums { get; }
        public InterfaceCollection Interfaces { get; }
    }
}
