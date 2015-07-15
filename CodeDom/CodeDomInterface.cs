using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.CodeModel.Collections;

namespace Typewriter.CodeModel.CodeDom
{
    public class CodeDomInterface : Interface
    {
        private readonly CodeInterface2 codeInterface;
        private readonly Item parent;

        private CodeDomInterface(CodeInterface2 codeInterface, Item parent)
        {
            this.codeInterface = codeInterface;
            this.parent = parent;
        }

        public Item Parent => parent;
        public string Name => codeInterface.Name;
        public string FullName => codeInterface.FullName;
        public string Namespace => codeInterface.Namespace.FullName;
        public bool IsGeneric => codeInterface.IsGeneric;

        private AttributeCollection attributes;
        public AttributeCollection Attributes => attributes ?? (attributes = new AttributeCollectionImpl(CodeDomAttribute.FromCodeElements(codeInterface.Attributes, this)));
        
        private TypeCollection genericTypeArguments;
        public TypeCollection GenericTypeArguments => genericTypeArguments ?? (genericTypeArguments = new TypeCollectionImpl(GenericTypeInfo.FromFullName(codeInterface.FullName, this)));

        private InterfaceCollection interfaces;
        public InterfaceCollection Interfaces => interfaces ?? (interfaces = new InterfaceCollectionImpl(FromCodeElements(codeInterface.Bases, this)));

        private MethodCollection methods;
        public MethodCollection Methods => methods ?? (methods = new MethodCollectionImpl(CodeDomMethod.FromCodeElements(codeInterface.Children, this)));

        private PropertyCollection properties;
        public PropertyCollection Properties => properties ?? (properties = new PropertyCollectionImpl(CodeDomProperty.FromCodeElements(codeInterface.Children, this)));

        private Class containingClass;
        public Class ContainingClass => containingClass ?? (containingClass = CodeDomClass.FromCodeClass(codeInterface.Parent as CodeClass2, this));

        internal static IEnumerable<Interface> FromCodeElements(CodeElements codeElements, Item parent)
        {
            return codeElements.OfType<CodeInterface2>().Where(i => i.Access == vsCMAccess.vsCMAccessPublic).Select(i => new CodeDomInterface(i, parent));
        }
    }
}