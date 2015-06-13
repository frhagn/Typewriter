using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class MethodInfo : Method
    {
        private readonly CodeFunction2 codeFunction;
        private readonly Item parent;

        private MethodInfo(CodeFunction2 codeFunction, Item parent)
        {
            this.codeFunction = codeFunction;
            this.parent = parent;
        }

        public Item Parent => parent;
        public string Name => codeFunction.Name;
        public string FullName => codeFunction.FullName;

        public bool IsEnum => Type.IsEnum;
        public bool IsEnumerable => Type.IsEnumerable;
        public bool IsGeneric => Type.IsGeneric;
        public bool IsNullable => Type.IsNullable;
        public bool IsPrimitive => Type.IsPrimitive;

        private Attribute[] attributes;
        public ICollection<Attribute> Attributes => attributes ?? (attributes = AttributeInfo.FromCodeElements(codeFunction.Attributes, this).ToArray());
        
        private Parameter[] parameters;
        public ICollection<Parameter> Parameters => parameters ?? (parameters = ParameterInfo.FromCodeElements(codeFunction.Parameters, this).ToArray());

        private Type type;
        public Type Type => type ?? (type = TypeInfo.FromCodeElement(codeFunction, this));

        internal static IEnumerable<Method> FromCodeElements(CodeElements codeElements, Item parent)
        {
            return codeElements.OfType<CodeFunction2>().Where(f => f.FunctionKind != vsCMFunction.vsCMFunctionConstructor && f.Access == vsCMAccess.vsCMAccessPublic).Select(f => new MethodInfo(f, parent));
        }
    }
}