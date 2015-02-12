using System;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class MethodInfo : ItemInfo, Method
    {
        private readonly CodeFunction2 codeFunction;

        public MethodInfo(CodeFunction2 codeFunction, object parent, FileInfo file) : base(codeFunction, parent, file)
        {
            this.codeFunction = codeFunction;
        }
    }
}