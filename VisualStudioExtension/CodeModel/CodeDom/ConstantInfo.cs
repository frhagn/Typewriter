using System;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class ConstantInfo : ItemInfo, IConstantInfo
    {
        private readonly CodeVariable2 codeVariable;

        public ConstantInfo(CodeVariable2 codeVariable, FileInfo file) : base(codeVariable, file)
        {
            this.codeVariable = codeVariable;
        }
    }
}