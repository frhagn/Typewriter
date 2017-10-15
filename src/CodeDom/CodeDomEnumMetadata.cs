using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.CodeDom
{
    public class CodeDomEnumMetadata : IEnumMetadata
    {
        private readonly CodeEnum _codeEnum;
        private readonly CodeDomFileMetadata _file;

        private CodeDomEnumMetadata(CodeEnum codeEnum, CodeDomFileMetadata file)
        {
            _codeEnum = codeEnum;
            _file = file;
        }

        public string DocComment => _codeEnum.DocComment;
        public string Name => _codeEnum.Name;
        public string FullName => _codeEnum.FullName;
        public string Namespace => GetNamespace();
        public ITypeMetadata Type => new LazyCodeDomTypeMetadata(_codeEnum.FullName, false, false, _file);
        public IEnumerable<IAttributeMetadata> Attributes => CodeDomAttributeMetadata.FromCodeElements(_codeEnum.Attributes);
        public IEnumerable<IEnumValueMetadata> Values => CodeDomEnumValueMetadata.FromCodeElements(_codeEnum.Members, _file);
        public IClassMetadata ContainingClass => CodeDomClassMetadata.FromCodeClass(_codeEnum.Parent as CodeClass2, _file);

        private string GetNamespace()
        {
            var parent = _codeEnum.Parent as CodeClass2;
            return parent != null ? parent.FullName : (_codeEnum.Namespace?.FullName ?? string.Empty);
        }

        internal static IEnumerable<IEnumMetadata> FromCodeElements(CodeElements codeElements, CodeDomFileMetadata file)
        {
            return codeElements.OfType<CodeEnum>().Where(e => e.Access == vsCMAccess.vsCMAccessPublic).Select(e => new CodeDomEnumMetadata(e, file));
        }
    }
}