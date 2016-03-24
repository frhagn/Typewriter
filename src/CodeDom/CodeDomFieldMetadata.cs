using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.CodeDom
{
    public class CodeDomFieldMetadata : IFieldMetadata
    {
        private readonly CodeVariable2 codeVariable;
        private readonly CodeDomFileMetadata file;

        protected CodeDomFieldMetadata(CodeVariable2 codeVariable, CodeDomFileMetadata file)
        {
            this.codeVariable = codeVariable;
            this.file = file;
        }

        public string DocComment => codeVariable.DocComment;
        public string Name => codeVariable.Name;
        public string FullName => codeVariable.FullName;
        public IEnumerable<IAttributeMetadata> Attributes => CodeDomAttributeMetadata.FromCodeElements(codeVariable.Attributes);
        public ITypeMetadata Type => CodeDomTypeMetadata.FromCodeElement(codeVariable, file);

        internal static IEnumerable<IFieldMetadata> FromCodeElements(CodeElements codeElements, CodeDomFileMetadata file)
        {
            return codeElements.OfType<CodeVariable2>().Where(v => v.Access == vsCMAccess.vsCMAccessPublic && v.IsConstant == false && v.IsShared == false).Select(v => new CodeDomFieldMetadata(v, file));
        }
    }
}