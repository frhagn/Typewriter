using System;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class PropertyInfo : ItemInfo, IPropertyInfo
    {
        private readonly CodeProperty2 codeProperty;

        public PropertyInfo(CodeProperty2 codeProperty, FileInfo file) : base(codeProperty, file)
        {
            this.codeProperty = codeProperty;
        }

        public bool HasGetter
        {
            get { return codeProperty.Getter != null; }
        }

        public bool HasSetter
        {
            get { return codeProperty.Setter != null; }
        }
    }
}