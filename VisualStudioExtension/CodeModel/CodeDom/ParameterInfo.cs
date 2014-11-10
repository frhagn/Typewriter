using System;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class ParameterInfo : ItemInfo, IParameterInfo
    {
        private readonly CodeParameter2 codeParameter;

        public ParameterInfo(CodeParameter2 codeParameter, FileInfo file) : base(codeParameter, file)
        {
            this.codeParameter = codeParameter;
        }
    }
}