using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class EnumInfo : Enum
    {
        private readonly CodeEnum codeEnum;
        private readonly Item parent;

        private EnumInfo(CodeEnum codeEnum, Item parent)
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

        private Attribute[] attributes;
        public ICollection<Attribute> Attributes => attributes ?? (attributes = AttributeInfo.FromCodeElements(codeEnum.Attributes, this).ToArray());

        private EnumValue[] values;
        public ICollection<EnumValue> Values => values ?? (values = EnumValueInfo.FromCodeElements(codeEnum.Members, this).ToArray());

        private Class containingClass;
        public Class ContainingClass => containingClass ?? (containingClass = ClassInfo.FromCodeClass(codeEnum.Parent as CodeClass2, this));

        internal static IEnumerable<Enum> FromCodeElements(CodeElements codeElements, Item parent)
        {
            return codeElements.OfType<CodeEnum>().Where(e => e.Access == vsCMAccess.vsCMAccessPublic).Select(e => new EnumInfo(e, parent));
        }
    }
}