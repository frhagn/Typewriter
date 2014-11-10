using System;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class AttributeInfo : ItemInfo, IAttributeInfo
    {
        private readonly CodeAttribute2 codeAttribute;

        public AttributeInfo(CodeAttribute2 codeAttribute, FileInfo file) : base(codeAttribute, file)
        {
            this.codeAttribute = codeAttribute;
        }

        public string Value
        {
            get
            {
                var value = codeAttribute.Value;
                return value == null ? null : value.Trim('"');
            }
        }
    }
}