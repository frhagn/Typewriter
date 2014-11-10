using System;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class MethodInfo : ItemInfo, IMethodInfo
    {
        private readonly CodeFunction2 codeFunction;

        public MethodInfo(CodeFunction2 codeFunction, FileInfo file) : base(codeFunction, file)
        {
            this.codeFunction = codeFunction;
        }
    }
}