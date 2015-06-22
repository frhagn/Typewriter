using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class ClassInfo : Class
    {
        private readonly CodeClass2 codeClass;
        private readonly Item parent;

        private ClassInfo(CodeClass2 codeClass, Item parent)
        {
            this.codeClass = codeClass;
            this.parent = parent;
        }

        public Item Parent => parent;
        public string Name => codeClass.Name;
        public string FullName => codeClass.FullName;
        public string Namespace => codeClass.Namespace.FullName;
        public bool IsGeneric => codeClass.IsGeneric;

        private Class  baseClass;
        public Class BaseClass => baseClass ?? (baseClass = FromCodeElements(codeClass.Bases, this).FirstOrDefault());

        private Class containingClass;
        public Class ContainingClass => containingClass ?? (containingClass = ClassInfo.FromCodeClass(codeClass.Parent as CodeClass2, this));

        private Attribute[] attributes;
        public ICollection<Attribute> Attributes => attributes ?? (attributes = AttributeInfo.FromCodeElements(codeClass.Attributes, this).ToArray());

        private Constant[] constants;
        public ICollection<Constant> Constants => constants ?? (constants = ConstantInfo.FromCodeElements(codeClass.Children, this).ToArray());

        private Field[] fields;
        public ICollection<Field> Fields => fields ?? (fields = FieldInfo.FromCodeElements(codeClass.Children, this).ToArray());

        private Type[] genericTypeArguments;
        public ICollection<Type> GenericTypeArguments => genericTypeArguments ?? (genericTypeArguments = GenericTypeInfo.FromFullName(codeClass.FullName, this).ToArray());

        private Interface[] interfaces;
        public ICollection<Interface> Interfaces => interfaces ?? (interfaces = InterfaceInfo.FromCodeElements(codeClass.ImplementedInterfaces, this).ToArray());

        private Method[] methods;
        public ICollection<Method> Methods => methods ?? (methods = MethodInfo.FromCodeElements(codeClass.Children, this).ToArray());

        private Property[] properties;
        public ICollection<Property> Properties => properties ?? (properties = PropertyInfo.FromCodeElements(codeClass.Children, this).ToArray());

        private Class[] nestedClasses;
        public ICollection<Class> NestedClasses => nestedClasses ?? (nestedClasses = ClassInfo.FromCodeElements(codeClass.Members, this).ToArray());

        private Enum[] nestedEnums;
        public ICollection<Enum> NestedEnums => nestedEnums ?? (nestedEnums = EnumInfo.FromCodeElements(codeClass.Members, this).ToArray());

        private Interface[] nestedInterfaces;
        public ICollection<Interface> NestedInterfaces => nestedInterfaces ?? (nestedInterfaces = InterfaceInfo.FromCodeElements(codeClass.Members, this).ToArray());

        internal static IEnumerable<Class> FromCodeElements(CodeElements codeElements, Item parent)
        {
            return codeElements.OfType<CodeClass2>().Where(c => c.Access == vsCMAccess.vsCMAccessPublic && c.FullName != "System.Object").Select(c => new ClassInfo(c, parent));
        }

        internal static Class FromCodeClass(CodeClass2 codeClass, Item parent)
        {
            return codeClass == null ? null : new ClassInfo(codeClass, parent);
        }
    }
}