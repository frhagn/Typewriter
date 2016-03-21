using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.CodeDom
{
    public class CodeDomConstantMetadata : CodeDomFieldMetadata, IConstantMetadata
    {
        private readonly string value;

        private CodeDomConstantMetadata(CodeVariable2 codeVariable, CodeDomFileMetadata file, string value) : base(codeVariable, file)
        {
            this.value = value;
        }
        
        public string Value => value;

        internal new static IEnumerable<IConstantMetadata> FromCodeElements(CodeElements codeElements, CodeDomFileMetadata file)
        {
            return codeElements
                .OfType<CodeVariable2>()
                .Where(v => v.IsConstant && v.Access == vsCMAccess.vsCMAccessPublic)
                .Select(v =>
                {
                    var initValue = (string)v.InitExpression.ToString();
                    var constantValue = initValue == "null" ? string.Empty : $"{initValue}".Trim('"');
                    return new CodeDomConstantMetadata(v, file, constantValue);
                });
        }
    }
}