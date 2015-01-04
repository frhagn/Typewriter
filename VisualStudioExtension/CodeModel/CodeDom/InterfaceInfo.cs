using System;
using System.Collections.Generic;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class InterfaceInfo : ItemInfo, IInterfaceInfo
    {
        private readonly CodeInterface2 codeInterface;

        public InterfaceInfo(CodeInterface2 codeInterface, FileInfo file) : base(codeInterface, file)
        {
            this.codeInterface = codeInterface;
        }

        public override bool IsGeneric
        {
            get { return codeInterface.IsGeneric; }
        }
    }
}