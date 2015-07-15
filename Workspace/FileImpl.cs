using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Typewriter.CodeModel.Workspace
{
    public class FileImpl : File
    {
        private readonly Document document;
        private ClassDeclarationSyntax[] classes;

        public FileImpl(Document document)
        {
            this.document = document;
            var semanticModel = document.GetSemanticModelAsync().Result;
            var root = semanticModel.SyntaxTree.GetRoot();
            classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToArray();
        }

        public string Name => document.Name;
        public string FullName => classes.First().Identifier.ToString();
        public ClassCollection Classes { get; }
        public EnumCollection Enums { get; }
        public InterfaceCollection Interfaces { get; }
    }
}
