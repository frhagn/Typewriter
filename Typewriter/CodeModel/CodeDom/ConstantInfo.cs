using System;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class ConstantInfo : ItemInfo, Constant
    {
        private readonly CodeVariable2 codeVariable;

        public ConstantInfo(CodeVariable2 codeVariable, object parent, FileInfo file) : base(codeVariable, parent, file)
        {
            this.codeVariable = codeVariable;
        }
    }
}