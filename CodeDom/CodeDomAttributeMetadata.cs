using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.CodeDom
{
    public class CodeDomAttributeMetadata : IAttributeMetadata
    {
        private readonly CodeAttribute2 codeAttribute;
        private readonly CodeDomFileMetadata file;

        private CodeDomAttributeMetadata(CodeAttribute2 codeAttribute, CodeDomFileMetadata file)
        {
            this.codeAttribute = codeAttribute;
            this.file = file;
        }

        public string Name => codeAttribute.Name;
        public string FullName => codeAttribute.FullName;
        public string Value => codeAttribute.Value?.Trim('"');

        internal static IEnumerable<IAttributeMetadata> FromCodeElements(CodeElements codeElements, CodeDomFileMetadata file)
        {
            return codeElements.OfType<CodeAttribute2>().Select(a => new CodeDomAttributeMetadata(a, file));
        }
    }
}