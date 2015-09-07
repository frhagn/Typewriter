using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.CodeDom
{
    public class CodeDomEnumMetadata : IEnumMetadata
    {
        private readonly CodeEnum codeEnum;
        private readonly CodeDomFileMetadata file;

        private CodeDomEnumMetadata(CodeEnum codeEnum, CodeDomFileMetadata file)
        {
            this.codeEnum = codeEnum;
            this.file = file;
        }

        public string Name => codeEnum.Name;
        public string FullName => codeEnum.FullName;
        public string Namespace => GetNamespace();
        public IEnumerable<IAttributeMetadata> Attributes => CodeDomAttributeMetadata.FromCodeElements(codeEnum.Attributes);
        public IEnumerable<IEnumValueMetadata> Values => CodeDomEnumValueMetadata.FromCodeElements(codeEnum.Members, file);
        public IClassMetadata ContainingClass => CodeDomClassMetadata.FromCodeClass(codeEnum.Parent as CodeClass2, file);

        private string GetNamespace()
        {
            var parent = codeEnum.Parent as CodeClass2;
            return parent != null ? parent.FullName : codeEnum.Namespace.FullName;
        }

        internal static IEnumerable<IEnumMetadata> FromCodeElements(CodeElements codeElements, CodeDomFileMetadata file)
        {
            return codeElements.OfType<CodeEnum>().Where(e => e.Access == vsCMAccess.vsCMAccessPublic).Select(e => new CodeDomEnumMetadata(e, file));
        }
    }
}