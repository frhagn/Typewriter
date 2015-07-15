using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.CodeModel.Collections;

namespace Typewriter.CodeModel.CodeDom
{
    public class CodeDomClass : Class
    {
        private readonly CodeClass2 codeClass;
        private readonly Item parent;

        private CodeDomClass(CodeClass2 codeClass, Item parent)
        {
            this.codeClass = codeClass;
            this.parent = parent;
        }

        public Item Parent => parent;
        public string Name => codeClass.Name;
        public string FullName => codeClass.FullName;
        public string Namespace => codeClass.Namespace.FullName;
        public bool IsGeneric => codeClass.IsGeneric;

        private Class baseClass;
        public Class BaseClass => baseClass ?? (baseClass = FromCodeElements(codeClass.Bases, this).FirstOrDefault());

        private Class containingClass;
        public Class ContainingClass => containingClass ?? (containingClass = CodeDomClass.FromCodeClass(codeClass.Parent as CodeClass2, this));

        private AttributeCollection attributes;
        public AttributeCollection Attributes => attributes ?? (attributes = new AttributeCollectionImpl(CodeDomAttribute.FromCodeElements(codeClass.Attributes, this)));

        private ConstantCollection constants;
        public ConstantCollection Constants => constants ?? (constants = new ConstantCollectionImpl(CodeDomConstant.FromCodeElements(codeClass.Children, this)));

        private FieldCollection fields;
        public FieldCollection Fields => fields ?? (fields = new FieldCollectionImpl(CodeDomField.FromCodeElements(codeClass.Children, this)));

        private TypeCollection genericTypeArguments;
        public TypeCollection GenericTypeArguments => genericTypeArguments ?? (genericTypeArguments = new TypeCollectionImpl(GenericTypeInfo.FromFullName(codeClass.FullName, this)));

        private InterfaceCollection interfaces;
        public InterfaceCollection Interfaces => interfaces ?? (interfaces = new InterfaceCollectionImpl(CodeDomInterface.FromCodeElements(codeClass.ImplementedInterfaces, this)));

        private MethodCollection methods;
        public MethodCollection Methods => methods ?? (methods = new MethodCollectionImpl(CodeDomMethod.FromCodeElements(codeClass.Children, this)));

        private PropertyCollection properties;
        public PropertyCollection Properties => properties ?? (properties = new PropertyCollectionImpl(CodeDomProperty.FromCodeElements(codeClass.Children, this)));

        private ClassCollection nestedClasses;
        public ClassCollection NestedClasses => nestedClasses ?? (nestedClasses = new ClassCollectionImpl(CodeDomClass.FromCodeElements(codeClass.Members, this)));

        private EnumCollection nestedEnums;
        public EnumCollection NestedEnums => nestedEnums ?? (nestedEnums = new EnumCollectionImpl(CodeDomEnum.FromCodeElements(codeClass.Members, this)));

        private InterfaceCollection nestedInterfaces;
        public InterfaceCollection NestedInterfaces => nestedInterfaces ?? (nestedInterfaces = new InterfaceCollectionImpl(CodeDomInterface.FromCodeElements(codeClass.Members, this)));

        internal static IEnumerable<Class> FromCodeElements(CodeElements codeElements, Item parent)
        {
            return codeElements.OfType<CodeClass2>().Where(c => c.Access == vsCMAccess.vsCMAccessPublic && c.FullName != "System.Object").Select(c => new CodeDomClass(c, parent));
        }

        internal static Class FromCodeClass(CodeClass2 codeClass, Item parent)
        {
            return codeClass == null ? null : new CodeDomClass(codeClass, parent);
        }
    }
}