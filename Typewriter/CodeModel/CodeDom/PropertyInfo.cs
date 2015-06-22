using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class PropertyInfo : Property
    {
        private readonly CodeProperty2 codeProperty;
        private readonly Item parent;

        private PropertyInfo(CodeProperty2 codeProperty, Item parent)
        {
            this.codeProperty = codeProperty;
            this.parent = parent;
        }

        public Item Parent => parent;
        public string Name => codeProperty.Name;
        public string FullName => codeProperty.FullName;

        public bool HasGetter => codeProperty.Getter != null && codeProperty.Getter.Access == vsCMAccess.vsCMAccessPublic;
        public bool HasSetter => codeProperty.Setter != null && codeProperty.Setter.Access == vsCMAccess.vsCMAccessPublic;

        //public bool IsEnum => Type.IsEnum;
        //public bool IsEnumerable => Type.IsEnumerable;
        //public bool IsGeneric => Type.IsGeneric;
        //public bool IsNullable => Type.IsNullable;
        //public bool IsPrimitive => Type.IsPrimitive;

        private Attribute[] attributes;
        public ICollection<Attribute> Attributes => attributes ?? (attributes = AttributeInfo.FromCodeElements(codeProperty.Attributes, this).ToArray());

        private Type type;
        public Type Type => type ?? (type = TypeInfo.FromCodeElement(codeProperty, this));
        
        internal static IEnumerable<Property> FromCodeElements(CodeElements codeElements, Item parent)
        {
            return codeElements.OfType<CodeProperty2>().Where(p => p.Access == vsCMAccess.vsCMAccessPublic && p.IsShared == false).Select(p => new PropertyInfo(p, parent));
        }
    }
}