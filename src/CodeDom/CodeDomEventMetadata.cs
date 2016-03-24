using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.CodeDom
{
    public class CodeDomEventMetadata : IEventMetadata
    {
        private readonly CodeEvent codeEvent;
        private readonly CodeDomFileMetadata file;

        private CodeDomEventMetadata(CodeEvent codeEvent, CodeDomFileMetadata file)
        {
            this.codeEvent = codeEvent;
            this.file = file;
        }

        public string DocComment => codeEvent.DocComment;
        public string Name => codeEvent.Name;
        public string FullName => codeEvent.FullName;
        public IEnumerable<IAttributeMetadata> Attributes => CodeDomAttributeMetadata.FromCodeElements(codeEvent.Attributes);
        public ITypeMetadata Type => CodeDomTypeMetadata.FromCodeElement(codeEvent, file);

        internal static IEnumerable<IEventMetadata> FromCodeElements(CodeElements codeElements, CodeDomFileMetadata file)
        {
            return codeElements.OfType<CodeEvent>().Where(f => f.Access == vsCMAccess.vsCMAccessPublic && f.IsShared == false).Select(f => new CodeDomEventMetadata(f, file));
        }
    }
}
