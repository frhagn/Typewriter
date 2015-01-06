using System;
using System.Collections.Generic;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class ClassInfo : ItemInfo, IClassInfo
    {
        private readonly CodeClass2 codeClass;

        public ClassInfo(CodeClass2 codeClass, object parent, FileInfo file) : base(codeClass, parent, file)
        {
            this.codeClass = codeClass;
        }

        public override bool IsGeneric
        {
            get { return codeClass.IsGeneric; }
        }
    }
}