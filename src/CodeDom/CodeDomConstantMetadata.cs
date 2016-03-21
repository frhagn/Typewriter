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
            var initValue = codeVariable.InitExpression.ToString();
            var value = initValue == "null" ? string.Empty : initValue;

            if (value.Length >= 2 && value.StartsWith("\"") && value.EndsWith("\""))
            {
                Value = value.Substring(1, value.Length - 2).Replace("\\\"", "\"");
            }
            else
            {
                Value = value;
            }
        }
        
        public string Value { get; }

        internal new static IEnumerable<IConstantMetadata> FromCodeElements(CodeElements codeElements, CodeDomFileMetadata file)
        {
            return codeElements.OfType<CodeVariable2>().Where(v => v.IsConstant && v.Access == vsCMAccess.vsCMAccessPublic)
                .Select(v => new CodeDomConstantMetadata(v, file));
        }
    }
}