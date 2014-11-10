using System;
using System.Collections.Generic;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class ClassInfo : ItemInfo, IClassInfo
    {
        private readonly CodeClass2 codeClass;

        public ClassInfo(CodeClass2 codeClass, FileInfo file) : base(codeClass, file)
        {
            this.codeClass = codeClass;
        }

        public bool IsGeneric
        {
            get { return codeClass.IsGeneric; }
        }
    }
}