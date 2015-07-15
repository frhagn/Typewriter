using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.CodeModel.Collections;

namespace Typewriter.CodeModel.CodeDom
{
    public class CodeDomProperty : Property
    {
        private readonly CodeProperty2 codeProperty;
        private readonly Item parent;

        private CodeDomProperty(CodeProperty2 codeProperty, Item parent)
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

        private AttributeCollection attributes;
        public AttributeCollection Attributes => attributes ?? (attributes = new AttributeCollectionImpl(CodeDomAttribute.FromCodeElements(codeProperty.Attributes, this)));

        private Type type;
        public Type Type => type ?? (type = CodeDomType.FromCodeElement(codeProperty, this));
        
        internal static IEnumerable<Property> FromCodeElements(CodeElements codeElements, Item parent)
        {
            return codeElements.OfType<CodeProperty2>().Where(p => p.Access == vsCMAccess.vsCMAccessPublic && p.IsShared == false).Select(p => new CodeDomProperty(p, parent));
        }
    }
}