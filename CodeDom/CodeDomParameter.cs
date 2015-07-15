using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.CodeModel.Collections;

namespace Typewriter.CodeModel.CodeDom
{
    public class CodeDomParameter : Parameter
    {
        private readonly CodeParameter2 codeParameter;
        private readonly Item parent;

        private CodeDomParameter(CodeParameter2 codeParameter, Item parent)
        {
            this.codeParameter = codeParameter;
            this.parent = parent;
        }

        public Item Parent => parent;
        public string Name => codeParameter.Name;
        public string FullName => codeParameter.FullName;

        //public bool IsEnum => Type.IsEnum;
        //public bool IsEnumerable => Type.IsEnumerable;
        //public bool IsGeneric => Type.IsGeneric;
        //public bool IsNullable => Type.IsNullable;
        //public bool IsPrimitive => Type.IsPrimitive;

        private AttributeCollection attributes;
        public AttributeCollection Attributes => attributes ?? (attributes = new AttributeCollectionImpl(CodeDomAttribute.FromCodeElements(codeParameter.Attributes, this)));

        private Type type;
        public Type Type => type ?? (type = CodeDomType.FromCodeElement(codeParameter, this));

        internal static IEnumerable<Parameter> FromCodeElements(CodeElements codeElements, Item parent)
        {
            return codeElements.OfType<CodeParameter2>().Select(p => new CodeDomParameter(p, parent));
        }
    }
}