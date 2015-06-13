using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class AttributeInfo : Attribute
    {
        private readonly CodeAttribute2 codeAttribute;
        private readonly Item parent;

        private AttributeInfo(CodeAttribute2 codeAttribute, Item parent)
        {
            this.codeAttribute = codeAttribute;
            this.parent = parent;
        }

        public Item Parent => parent;
        public string Name => codeAttribute.Name;
        public string FullName => codeAttribute.FullName;
        public string Value => codeAttribute.Value?.Trim('"');

        private Attribute[] attributes;
        public ICollection<Attribute> Attributes => attributes ?? (attributes = AttributeInfo.FromCodeElements(codeAttribute.Children, this).ToArray());
        
        public static IEnumerable<Attribute> FromCodeElements(CodeElements codeElements, Item parent)
        {
            return codeElements.OfType<CodeAttribute2>().Select(a => new AttributeInfo(a, parent));
        }
    }
}