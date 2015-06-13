using System.Collections.Generic;
using System.Linq;
using EnvDTE;

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

        private Attribute[] attributes;
        public ICollection<Attribute> Attributes => attributes ?? (attributes = AttributeInfo.FromCodeElements(codeEnum.Attributes, this).ToArray());

        private EnumValue[] values;
        public ICollection<EnumValue> Values => values ?? (values = EnumValueInfo.FromCodeElements(codeEnum.Members, this).ToArray());

        internal static IEnumerable<Enum> FromCodeElements(CodeElements codeElements, Item parent)
        {
            return codeElements.OfType<CodeEnum>().Where(e => e.Access == vsCMAccess.vsCMAccessPublic).Select(e => new EnumInfo(e, parent));
        }
    }
}