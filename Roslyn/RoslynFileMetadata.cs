using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynFileMetadata : IFileMetadata
    {
        private readonly Document document;
        private readonly SyntaxNode root;

        public RoslynFileMetadata(Document document)
        {
            this.document = document;
            var semanticModel = document.GetSemanticModelAsync().Result;
            root = semanticModel.SyntaxTree.GetRoot();
        }

        public string Name => document.Name;
        public string FullName => document.FilePath;
        
        public IEnumerable<IClassMetadata> Classes => RoslynClassMetadata.FromClassSyntax(root.ChildNodes().OfType<NamespaceDeclarationSyntax>().SelectMany(n => n.ChildNodes().OfType<ClassDeclarationSyntax>()));

        public IEnumerable<IEnumMetadata> Enums { get; }
        public IEnumerable<IInterfaceMetadata> Interfaces { get; }
    }
}
