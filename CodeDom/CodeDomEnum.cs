using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.CodeModel.Collections;

namespace Typewriter.CodeModel.CodeDom
{
    public class CodeDomEnum : Enum
    {
        private readonly CodeEnum codeEnum;
        private readonly Item parent;

        private CodeDomEnum(CodeEnum codeEnum, Item parent)
        {
            this.codeEnum = codeEnum;
            this.parent = parent;
        }

        public Item Parent => parent;
        public string Name => codeEnum.Name;
        public string FullName => codeEnum.FullName;
        public string Namespace => codeEnum.Namespace.FullName;

        private bool? isFlags;
        public bool IsFlags => isFlags ?? (isFlags = Attributes.Any(a => a.FullName == "System.FlagsAttribute")).Value;

        private AttributeCollection attributes;
        public AttributeCollection Attributes => attributes ?? (attributes = new AttributeCollectionImpl(CodeDomAttribute.FromCodeElements(codeEnum.Attributes, this)));

        private EnumValueCollection values;
        public EnumValueCollection Values => values ?? (values = new EnumValueCollectionImpl(CodeDomEnumValue.FromCodeElements(codeEnum.Members, this)));

        private Class containingClass;
        public Class ContainingClass => containingClass ?? (containingClass = CodeDomClass.FromCodeClass(codeEnum.Parent as CodeClass2, this));

        internal static IEnumerable<Enum> FromCodeElements(CodeElements codeElements, Item parent)
        {
            return codeElements.OfType<CodeEnum>().Where(e => e.Access == vsCMAccess.vsCMAccessPublic).Select(e => new CodeDomEnum(e, parent));
        }
    }
}