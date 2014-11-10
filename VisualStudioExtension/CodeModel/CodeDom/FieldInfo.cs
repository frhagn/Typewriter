using System;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class FieldInfo : ItemInfo, IFieldInfo
    {
        private readonly CodeVariable2 codeVariable;

        public FieldInfo(CodeVariable2 codeVariable, FileInfo file) : base(codeVariable, file)
        {
            this.codeVariable = codeVariable;
        }
    }
}