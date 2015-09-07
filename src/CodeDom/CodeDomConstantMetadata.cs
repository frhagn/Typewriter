using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.CodeDom
{
    public class CodeDomConstantMetadata : CodeDomFieldMetadata, IConstantMetadata
    {
        private CodeDomConstantMetadata(CodeVariable2 codeVariable, CodeDomFileMetadata file) : base(codeVariable, file)
        {
        }

        internal new static IEnumerable<IConstantMetadata> FromCodeElements(CodeElements codeElements, CodeDomFileMetadata file)
        {
            return codeElements.OfType<CodeVariable2>().Where(v => v.IsConstant && v.Access == vsCMAccess.vsCMAccessPublic).Select(v => new CodeDomConstantMetadata(v, file));
        }
    }
}