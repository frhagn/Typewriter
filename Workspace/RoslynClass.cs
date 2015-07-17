using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Typewriter.CodeModel.Roslyn
{
    public class RoslynClass : Class
    {
        private readonly ClassDeclarationSyntax classSyntax;
        private readonly Item parent;

        private RoslynClass(ClassDeclarationSyntax classSyntax, Item parent)
        {
            this.classSyntax = classSyntax;
            this.parent = parent;
        }

        public string Name => classSyntax.Identifier.Text;
        public string FullName { get; }
        public Item Parent => parent;
        public AttributeCollection Attributes { get; }
        public string Namespace { get; }
        public bool IsGeneric { get; }
        public ConstantCollection Constants { get; }
        public FieldCollection Fields { get; }
        public Class BaseClass { get; }
        public Class ContainingClass { get; }
        public InterfaceCollection Interfaces { get; }
        public MethodCollection Methods { get; }
        public PropertyCollection Properties { get; }
        public TypeCollection GenericTypeArguments { get; }
        public ClassCollection NestedClasses { get; }
        public EnumCollection NestedEnums { get; }
        public InterfaceCollection NestedInterfaces { get; }

        internal static IEnumerable<Class> FromClassSyntax(IEnumerable<ClassDeclarationSyntax> classSyntax, Item parent)
        {
            return classSyntax.Where(c => c.Modifiers.Any(m => m.Kind() == SyntaxKind.PublicKeyword)).Select(c => new RoslynClass(c, parent));
        }
    }
}
