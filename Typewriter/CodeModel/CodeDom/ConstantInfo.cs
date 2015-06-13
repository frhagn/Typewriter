using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class ConstantInfo : FieldInfo, Constant
    {
        private ConstantInfo(CodeVariable2 codeVariable, Item parent) : base(codeVariable, parent)
        {
        }

        internal new static IEnumerable<Constant> FromCodeElements(CodeElements codeElements, Item parent)
        {
            return codeElements.OfType<CodeVariable2>().Where(v => v.IsConstant && v.Access == vsCMAccess.vsCMAccessPublic).Select(v => new ConstantInfo(v, parent));
        }
    }
}