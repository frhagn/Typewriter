using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.CodeDom
{
    public class CodeDomInterfaceMetadata : IInterfaceMetadata
    {
        private readonly CodeInterface2 _codeInterface;
        private readonly CodeDomFileMetadata _file;

        private CodeDomInterfaceMetadata(CodeInterface2 codeInterface, CodeDomFileMetadata file)
        {
            _codeInterface = codeInterface;
            _file = file;
        }

        public string DocComment => _codeInterface.DocComment;
        public string Name => _codeInterface.Name;
        public string FullName => _codeInterface.FullName;
        public string Namespace => GetNamespace();
        public bool IsGeneric => _codeInterface.FullName.EndsWith(">");

        public ITypeMetadata Type => new LazyCodeDomTypeMetadata(_codeInterface.FullName, false, false, _file);
        public IEnumerable<IAttributeMetadata> Attributes => CodeDomAttributeMetadata.FromCodeElements(_codeInterface.Attributes);
        public IEnumerable<IEventMetadata> Events => CodeDomEventMetadata.FromCodeElements(_codeInterface.Children, _file);
        public IEnumerable<ITypeParameterMetadata> TypeParameters => CodeDomTypeParameterMetadata.FromFullName(_codeInterface.FullName);
        public IEnumerable<ITypeMetadata> TypeArguments => CodeDomTypeMetadata.LoadGenericTypeArguments(IsGeneric, FullName, _file);
        public IEnumerable<IInterfaceMetadata> Interfaces => FromCodeElements(_codeInterface.Bases, _file);
        public IEnumerable<IMethodMetadata> Methods => CodeDomMethodMetadata.FromCodeElements(_codeInterface.Children, _file);
        public IEnumerable<IPropertyMetadata> Properties => CodeDomPropertyMetadata.FromCodeElements(_codeInterface.Children, _file);
        public IClassMetadata ContainingClass => CodeDomClassMetadata.FromCodeClass(_codeInterface.Parent as CodeClass2, _file);

        private string GetNamespace()
        {
            var parent = _codeInterface.Parent as CodeClass2;
            return parent != null ? parent.FullName : (_codeInterface.Namespace?.FullName ?? string.Empty);
        }

        internal static IEnumerable<IInterfaceMetadata> FromCodeElements(CodeElements codeElements, CodeDomFileMetadata file)
        {
            return codeElements.OfType<CodeInterface2>().Where(i => i.Access == vsCMAccess.vsCMAccessPublic).Select(i => new CodeDomInterfaceMetadata(i, file));
        }
    }
}