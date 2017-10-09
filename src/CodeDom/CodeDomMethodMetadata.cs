using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.CodeDom
{
    public class CodeDomMethodMetadata : IMethodMetadata
    {
        private readonly CodeFunction2 codeFunction;
        private readonly CodeDomFileMetadata file;

        private CodeDomMethodMetadata(CodeFunction2 codeFunction, CodeDomFileMetadata file)
        {
            this.codeFunction = codeFunction;
            this.file = file;
        }

        public string DocComment => codeFunction.DocComment;
        public string Name => codeFunction.Name;
        public string FullName => codeFunction.FullName;
        public bool IsGeneric => codeFunction.IsGeneric;
        public bool IsAbstract => codeFunction.MustImplement;
        public IEnumerable<IAttributeMetadata> Attributes => CodeDomAttributeMetadata.FromCodeElements(codeFunction.Attributes);
        public IEnumerable<ITypeParameterMetadata> TypeParameters => CodeDomTypeParameterMetadata.FromFullName(GetFullMethodName());
        public IEnumerable<IParameterMetadata> Parameters => CodeDomParameterMetadata.FromCodeElements(codeFunction.Parameters, file);
        public ITypeMetadata Type => CodeDomTypeMetadata.FromCodeElement(codeFunction, file);

        private string GetFullMethodName()
        {
            var index = 0;

            var parentClass = codeFunction.Parent as CodeClass2;
            if (parentClass != null)
            {
                index = parentClass.FullName.Length + 1;
            }
            else
            {
                var parentInterface = codeFunction.Parent as CodeInterface2;
                if (parentInterface != null)
                {
                    index = parentInterface.FullName.Length + 1;
                }
            }

            return codeFunction.FullName.Remove(0, index);
        }

        internal static IEnumerable<IMethodMetadata> FromCodeElements(CodeElements codeElements, CodeDomFileMetadata file)
        {
            return codeElements.OfType<CodeFunction2>().Where(f => f.Access == vsCMAccess.vsCMAccessPublic && f.FunctionKind != vsCMFunction.vsCMFunctionConstructor && f.IsShared == false).Select(f => new CodeDomMethodMetadata(f, file));
        }
    }
}