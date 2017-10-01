using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.CodeDom
{
    public class CodeDomPropertyMetadata : IPropertyMetadata
    {
        private readonly CodeProperty2 codeProperty;
        private readonly CodeDomFileMetadata file;

        private CodeDomPropertyMetadata(CodeProperty2 codeProperty, CodeDomFileMetadata file)
        {
            this.codeProperty = codeProperty;
            this.file = file;
        }

        public string DocComment => codeProperty.DocComment;
        public string Name => codeProperty.Name;
        public string FullName => codeProperty.FullName;
        public bool HasGetter => codeProperty.Getter != null && codeProperty.Getter.Access == vsCMAccess.vsCMAccessPublic;
        public bool HasSetter => codeProperty.Setter != null && codeProperty.Setter.Access == vsCMAccess.vsCMAccessPublic;
        public bool IsAbstract => 
            (codeProperty.Getter != null && codeProperty.Getter.MustImplement) || 
            (codeProperty.Setter != null && codeProperty.Setter.MustImplement);
        public IEnumerable<IAttributeMetadata> Attributes => CodeDomAttributeMetadata.FromCodeElements(codeProperty.Attributes);
        public ITypeMetadata Type => CodeDomTypeMetadata.FromCodeElement(codeProperty, file);
        
        internal static IEnumerable<IPropertyMetadata> FromCodeElements(CodeElements codeElements, CodeDomFileMetadata file)
        {
            return codeElements.OfType<CodeProperty2>().Where(p => p.Access == vsCMAccess.vsCMAccessPublic && p.IsShared == false).Select(p => new CodeDomPropertyMetadata(p, file));
        }
    }
}