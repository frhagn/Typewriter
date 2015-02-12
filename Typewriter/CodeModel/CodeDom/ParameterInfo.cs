using System;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class ParameterInfo : ItemInfo, Parameter
    {
        private readonly CodeParameter2 codeParameter;

        public ParameterInfo(CodeParameter2 codeParameter, object parent, FileInfo file) : base(codeParameter, parent, file)
        {
            this.codeParameter = codeParameter;
        }
    }
}