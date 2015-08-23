using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.CodeDom
{
    public class CodeDomInterfaceMetadata : IInterfaceMetadata
    {
        private readonly CodeInterface2 codeInterface;
        private readonly CodeDomFileMetadata file;

        private CodeDomInterfaceMetadata(CodeInterface2 codeInterface, CodeDomFileMetadata file)
        {
            this.codeInterface = codeInterface;
            this.file = file;
        }

        public string Name => codeInterface.Name;
        public string FullName => codeInterface.FullName;
        public string Namespace => GetNamespace();
        public bool IsGeneric => codeInterface.FullName.EndsWith(">");

        public IEnumerable<IAttributeMetadata> Attributes => CodeDomAttributeMetadata.FromCodeElements(codeInterface.Attributes);
        public IEnumerable<ITypeParameterMetadata> TypeParameters => CodeDomTypeParameterMetadata.FromFullName(codeInterface.FullName);
        public IEnumerable<ITypeMetadata> TypeArguments => CodeDomTypeMetadata.LoadGenericTypeArguments(IsGeneric, FullName, file);
        public IEnumerable<IInterfaceMetadata> Interfaces => CodeDomInterfaceMetadata.FromCodeElements(codeInterface.Bases, file);
        public IEnumerable<IMethodMetadata> Methods => CodeDomMethodMetadata.FromCodeElements(codeInterface.Children, file);
        public IEnumerable<IPropertyMetadata> Properties => CodeDomPropertyMetadata.FromCodeElements(codeInterface.Children, file);
        public IClassMetadata ContainingClass => CodeDomClassMetadata.FromCodeClass(codeInterface.Parent as CodeClass2, file);

        private string GetNamespace()
        {
            var parent = codeInterface.Parent as CodeClass2;
            return parent != null ? parent.FullName : codeInterface.Namespace.FullName;
        }

        internal static IEnumerable<IInterfaceMetadata> FromCodeElements(CodeElements codeElements, CodeDomFileMetadata file)
        {
            return codeElements.OfType<CodeInterface2>().Where(i => i.Access == vsCMAccess.vsCMAccessPublic).Select(i => new CodeDomInterfaceMetadata(i, file));
        }
    }
}