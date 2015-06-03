using System;
using System.Collections.Generic;
using EnvDTE80;
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

        public IEnumerable<Type> GenericTypeArguments
        {
            get
            {
                if (IsGeneric == false) return new Type[0];
                return ExtractGenericTypeNames(FullName).Select(n => new GenericTypeInfo(n, this, file));
            }
        }

        public override bool IsGeneric
        {
            get { return codeClass.IsGeneric; }
        }
    }
}