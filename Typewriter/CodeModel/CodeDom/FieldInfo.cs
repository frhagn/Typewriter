using System;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class FieldInfo : ItemInfo, Field
    {
        private readonly CodeVariable2 codeVariable;

        public FieldInfo(CodeVariable2 codeVariable, object parent, FileInfo file) : base(codeVariable, parent, file)
        {
            this.codeVariable = codeVariable;
        }
    }
}