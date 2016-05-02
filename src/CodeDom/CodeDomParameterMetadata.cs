using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.CodeDom
{
    public class CodeDomParameterMetadata : IParameterMetadata
    {
        private readonly CodeParameter2 codeParameter;
        private readonly CodeDomFileMetadata file;

        private CodeDomParameterMetadata(CodeParameter2 codeParameter, CodeDomFileMetadata file)
        {
            this.codeParameter = codeParameter;
            this.file = file;
        }

        public string Name => codeParameter.Name;
        public string FullName => codeParameter.FullName;
        public bool HasDefaultValue => codeParameter.DefaultValue != null;
        public string DefaultValue => codeParameter.DefaultValue;
        public IEnumerable<IAttributeMetadata> Attributes => CodeDomAttributeMetadata.FromCodeElements(codeParameter.Attributes);
        public ITypeMetadata Type => CodeDomTypeMetadata.FromCodeElement(codeParameter, file);

        internal static IEnumerable<IParameterMetadata> FromCodeElements(CodeElements codeElements, CodeDomFileMetadata file)
        {
            return codeElements.OfType<CodeParameter2>().Select(p => new CodeDomParameterMetadata(p, file));
        }
    }
}