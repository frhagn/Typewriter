using System;
using EnvDTE80;
using System.Collections.Generic;
using System.Linq;

namespace Typewriter.CodeModel.CodeDom
{
    public class ClassInfo : ItemInfo, Class
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

        public override bool IsNullable
        {
            get
            {
                return true;
            }
        }

    }
}