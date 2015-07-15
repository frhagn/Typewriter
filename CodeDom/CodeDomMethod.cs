using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.CodeModel.Collections;

namespace Typewriter.CodeModel.CodeDom
{
    public class CodeDomMethod : Method
    {
        private readonly CodeFunction2 codeFunction;
        private readonly Item parent;

        private CodeDomMethod(CodeFunction2 codeFunction, Item parent)
        {
            this.codeFunction = codeFunction;
            this.parent = parent;
        }

        public Item Parent => parent;
        public string Name => codeFunction.Name;
        public string FullName => codeFunction.FullName;

        //public bool IsEnum => Type.IsEnum;
        //public bool IsEnumerable => Type.IsEnumerable;
        public bool IsGeneric => codeFunction.IsGeneric;
        //public bool IsNullable => Type.IsNullable;
        //public bool IsPrimitive => Type.IsPrimitive;

        private AttributeCollection attributes;
        public AttributeCollection Attributes => attributes ?? (attributes = new AttributeCollectionImpl(CodeDomAttribute.FromCodeElements(codeFunction.Attributes, this)));

        private TypeCollection genericTypeArguments;
        public TypeCollection GenericTypeArguments => genericTypeArguments ?? (genericTypeArguments = new TypeCollectionImpl(GenericTypeInfo.FromFullName(codeFunction.FullName.Remove(0, parent.FullName.Length + 1), this)));

        private ParameterCollection parameters;
        public ParameterCollection Parameters => parameters ?? (parameters = new ParameterCollectionImpl(CodeDomParameter.FromCodeElements(codeFunction.Parameters, this)));

        private Type type;
        public Type Type => type ?? (type = CodeDomType.FromCodeElement(codeFunction, this));

        internal static IEnumerable<Method> FromCodeElements(CodeElements codeElements, Item parent)
        {
            return codeElements.OfType<CodeFunction2>().Where(f => f.Access == vsCMAccess.vsCMAccessPublic && f.FunctionKind != vsCMFunction.vsCMFunctionConstructor && f.IsShared == false).Select(f => new CodeDomMethod(f, parent));
        }
    }
}