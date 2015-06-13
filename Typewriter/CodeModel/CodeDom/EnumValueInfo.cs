using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class EnumValueInfo : EnumValue
    {
        private readonly CodeVariable2 codeVariable;
        private readonly Item parent;
        private readonly int index;

        private EnumValueInfo(CodeVariable2 codeVariable, Item parent, int index)
        {
            this.codeVariable = codeVariable;
            this.parent = parent;
            this.index = index;
        }

        public Item Parent => parent;
        public string Name => codeVariable.Name;
        public string FullName => codeVariable.FullName;

        private Attribute[] attributes;
        public ICollection<Attribute> Attributes => attributes ?? (attributes = AttributeInfo.FromCodeElements(codeVariable.Attributes, this).ToArray());

        public int Value => codeVariable.InitExpression == null ? index : int.Parse(codeVariable.InitExpression.ToString());

        internal static IEnumerable<EnumValue> FromCodeElements(CodeElements codeElements, Item parent)
        {
            return codeElements.OfType<CodeVariable2>().Select((v, i) => new EnumValueInfo(v, parent, i));
        }
    }
}