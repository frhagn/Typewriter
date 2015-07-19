using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.Roslyn
{
    public class RoslynClassMetadata : IClassMetadata
    {
        private readonly ClassDeclarationSyntax classSyntax;

        private RoslynClassMetadata(ClassDeclarationSyntax classSyntax)
        {
            this.classSyntax = classSyntax;
        }

        public string Name => classSyntax.Identifier.Text;
        public string FullName { get; }
        public bool IsGeneric { get; }
        public string Namespace { get; }
        public IEnumerable<IAttributeMetadata> Attributes { get; }
        public IClassMetadata BaseClass { get; }
        public IClassMetadata ContainingClass { get; }
        public IEnumerable<IConstantMetadata> Constants { get; }
        public IEnumerable<IFieldMetadata> Fields { get; }
        public IEnumerable<IInterfaceMetadata> Interfaces { get; }
        public IEnumerable<IMethodMetadata> Methods { get; }
        public IEnumerable<IPropertyMetadata> Properties { get; }
        public IEnumerable<ITypeMetadata> GenericTypeArguments { get; }
        public IEnumerable<IClassMetadata> NestedClasses { get; }
        public IEnumerable<IEnumMetadata> NestedEnums { get; }
        public IEnumerable<IInterfaceMetadata> NestedInterfaces { get; }

        internal static IEnumerable<IClassMetadata> FromClassSyntax(IEnumerable<ClassDeclarationSyntax> classSyntax)
        {
            return classSyntax.Where(c => c.Modifiers.Any(m => m.Kind() == SyntaxKind.PublicKeyword)).Select(c => new RoslynClassMetadata(c));
        }
    }
}
