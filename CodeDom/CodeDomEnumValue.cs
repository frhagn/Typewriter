using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.CodeModel.Collections;

namespace Typewriter.CodeModel.CodeDom
{
    public class CodeDomEnumValue : EnumValue
    {
        private readonly CodeVariable2 codeVariable;
        private readonly Item parent;
        private readonly int value;

        private CodeDomEnumValue(CodeVariable2 codeVariable, Item parent, int value)
        {
            this.codeVariable = codeVariable;
            this.parent = parent;
            this.value = value;
        }

        public Item Parent => parent;
        public string Name => codeVariable.Name;
        public string FullName => codeVariable.FullName;

        private AttributeCollection attributes;
        public AttributeCollection Attributes => attributes ?? (attributes = new AttributeCollectionImpl(CodeDomAttribute.FromCodeElements(codeVariable.Attributes, this)));

        public int Value => value;

        internal static IEnumerable<EnumValue> FromCodeElements(CodeElements codeElements, Item parent)
        {
            var value = -1;

            foreach (var codeVariable in codeElements.OfType<CodeVariable2>())
            {
                if (codeVariable.InitExpression == null)
                    value++;
                else
                    value = Convert.ToInt32(codeVariable.InitExpression);

                yield return new CodeDomEnumValue(codeVariable, parent, value);
            }
        }
    }
}