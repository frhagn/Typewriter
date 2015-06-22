using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class FieldInfo : Field
    {
        private readonly CodeVariable2 codeVariable;
        private readonly Item parent;

        protected FieldInfo(CodeVariable2 codeVariable, Item parent)
        {
            this.codeVariable = codeVariable;
            this.parent = parent;
        }

        public Item Parent => parent;
        public string Name => codeVariable.Name;
        public string FullName => codeVariable.FullName;

        //public bool IsEnum => Type.IsEnum;
        //public bool IsEnumerable => Type.IsEnumerable;
        //public bool IsGeneric => Type.IsGeneric;
        //public bool IsNullable => Type.IsNullable;
        //public bool IsPrimitive => Type.IsPrimitive;

        private Attribute[] attributes;
        public ICollection<Attribute> Attributes => attributes ?? (attributes = AttributeInfo.FromCodeElements(codeVariable.Attributes, this).ToArray());

        private Type type;
        public Type Type => type ?? (type = TypeInfo.FromCodeElement(codeVariable, this));

        internal static IEnumerable<Field> FromCodeElements(CodeElements codeElements, Item parent)
        {
            return codeElements.OfType<CodeVariable2>().Where(v => v.Access == vsCMAccess.vsCMAccessPublic && v.IsConstant == false && v.IsShared == false).Select(v => new FieldInfo(v, parent));
        }
    }
}