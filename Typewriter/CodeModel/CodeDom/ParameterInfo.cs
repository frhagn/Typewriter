using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class ParameterInfo : Parameter
    {
        private readonly CodeParameter2 codeParameter;
        private readonly Item parent;

        private ParameterInfo(CodeParameter2 codeParameter, Item parent)
        {
            this.codeParameter = codeParameter;
            this.parent = parent;
        }

        public Item Parent => parent;
        public string Name => codeParameter.Name;
        public string FullName => codeParameter.FullName;

        public bool IsEnum => Type.IsEnum;
        public bool IsEnumerable => Type.IsEnumerable;
        public bool IsGeneric => Type.IsGeneric;
        public bool IsNullable => Type.IsNullable;
        public bool IsPrimitive => Type.IsPrimitive;

        private Attribute[] attributes;
        public ICollection<Attribute> Attributes => attributes ?? (attributes = AttributeInfo.FromCodeElements(codeParameter.Attributes, this).ToArray());

        private Type type;
        public Type Type => type ?? (type = TypeInfo.FromCodeElement(codeParameter, this));

        internal static IEnumerable<Parameter> FromCodeElements(CodeElements codeElements, Item parent)
        {
            return codeElements.OfType<CodeParameter2>().Select(p => new ParameterInfo(p, parent));
        }
    }
}