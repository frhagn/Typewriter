using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.CodeDom
{
    public class CodeDomDelegateMetadata : IDelegateMetadata
    {
        private readonly CodeDelegate2 codeDelegate;
        private readonly CodeDomFileMetadata file;

        private CodeDomDelegateMetadata(CodeDelegate2 codeDelegate, CodeDomFileMetadata file)
        {
            this.codeDelegate = codeDelegate;
            this.file = file;
        }

        public string DocComment => codeDelegate.DocComment;
        public string Name => codeDelegate.Name;
        public string FullName => codeDelegate.FullName;
        public bool IsAbstract => false;
        public bool IsGeneric => codeDelegate.IsGeneric;
        public IEnumerable<IAttributeMetadata> Attributes => CodeDomAttributeMetadata.FromCodeElements(codeDelegate.Attributes);
        public IClassMetadata ContainingClass => CodeDomClassMetadata.FromCodeClass(codeDelegate.Parent as CodeClass2, file);
        public IEnumerable<ITypeParameterMetadata> TypeParameters => CodeDomTypeParameterMetadata.FromFullName(GetFullDelegateName());
        public IEnumerable<IParameterMetadata> Parameters => CodeDomParameterMetadata.FromCodeElements(codeDelegate.Parameters, file);
        public ITypeMetadata Type => CodeDomTypeMetadata.FromCodeElement(codeDelegate, file);

        private string GetFullDelegateName()
        {
            var index = 0;

            var parentClass = codeDelegate.Parent as CodeClass2;
            if (parentClass != null)
            {
                index = parentClass.FullName.Length + 1;
            }

            return codeDelegate.FullName.Remove(0, index);
        }

        internal static IDelegateMetadata FromCodeDelegate(CodeDelegate2 codeElement, CodeDomFileMetadata file)
        {
            return new CodeDomDelegateMetadata(codeElement, file);
        }

        internal static IEnumerable<IDelegateMetadata> FromCodeElements(CodeElements codeElements, CodeDomFileMetadata file)
        {
            return codeElements.OfType<CodeDelegate2>().Where(d => d.Access == vsCMAccess.vsCMAccessPublic).Select(d => FromCodeDelegate(d, file));
        }
    }
}