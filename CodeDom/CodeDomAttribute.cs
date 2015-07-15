using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.CodeModel.Collections;

namespace Typewriter.CodeModel.CodeDom
{
    public class CodeDomAttribute : Attribute
    {
        private readonly CodeAttribute2 codeAttribute;
        private readonly Item parent;

        private CodeDomAttribute(CodeAttribute2 codeAttribute, Item parent)
        {
            this.codeAttribute = codeAttribute;
            this.parent = parent;
        }

        public Item Parent => parent;
        public string Name => codeAttribute.Name;
        public string FullName => codeAttribute.FullName;
        public string Value => codeAttribute.Value?.Trim('"');

        private AttributeCollection attributes;
        public AttributeCollection Attributes => attributes ?? (attributes = new AttributeCollectionImpl(FromCodeElements(codeAttribute.Children, this)));

        internal static IEnumerable<Attribute> FromCodeElements(CodeElements codeElements, Item parent)
        {
            return codeElements.OfType<CodeAttribute2>().Select(a => new CodeDomAttribute(a, parent));
        }
    }
}