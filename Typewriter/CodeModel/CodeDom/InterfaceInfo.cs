using System;
using System.Collections.Generic;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class InterfaceInfo : ItemInfo, IInterfaceInfo
    {
        private readonly CodeInterface2 codeInterface;

        public InterfaceInfo(CodeInterface2 codeInterface, object parent, FileInfo file) : base(codeInterface, parent, file)
        {
            this.codeInterface = codeInterface;
        }

        public override bool IsGeneric
        {
            get { return codeInterface.IsGeneric; }
        }
    }
}