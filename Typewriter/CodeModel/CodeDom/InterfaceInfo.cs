using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class InterfaceInfo : Interface
    {
        private readonly CodeInterface2 codeInterface;
        private readonly Item parent;

        private InterfaceInfo(CodeInterface2 codeInterface, Item parent)
        {
            this.codeInterface = codeInterface;
            this.parent = parent;
        }

        public Item Parent => parent;
        public string Name => codeInterface.Name;
        public string FullName => codeInterface.FullName;
        public string Namespace => codeInterface.Namespace.FullName;
        public bool IsGeneric => codeInterface.IsGeneric;

        private Attribute[] attributes;
        public ICollection<Attribute> Attributes => attributes ?? (attributes = AttributeInfo.FromCodeElements(codeInterface.Attributes, this).ToArray());
        
        private Type[] genericTypeArguments;
        public ICollection<Type> GenericTypeArguments => genericTypeArguments ?? (genericTypeArguments = GenericTypeInfo.FromFullName(codeInterface.FullName, this).ToArray());

        private Interface[] interfaces;
        public ICollection<Interface> Interfaces => interfaces ?? (interfaces = FromCodeElements(codeInterface.Bases, this).ToArray());

        private Method[] methods;
        public ICollection<Method> Methods => methods ?? (methods = MethodInfo.FromCodeElements(codeInterface.Children, this).ToArray());

        private Property[] properties;
        public ICollection<Property> Properties => properties ?? (properties = PropertyInfo.FromCodeElements(codeInterface.Children, this).ToArray());

        private Class containingClass;
        public Class ContainingClass => containingClass ?? (containingClass = ClassInfo.FromCodeClass(codeInterface.Parent as CodeClass2, this));

        internal static IEnumerable<Interface> FromCodeElements(CodeElements codeElements, Item parent)
        {
            return codeElements.OfType<CodeInterface2>().Where(i => i.Access == vsCMAccess.vsCMAccessPublic).Select(i => new InterfaceInfo(i, parent));
        }
    }
}