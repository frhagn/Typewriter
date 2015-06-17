using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class TypeInfo : Type
    {
        private static readonly string[] primitiveTypes = { "System.Boolean", "System.Char", "System.String", "System.Byte", "System.SByte", "System.Int32", "System.UInt32", "System.Int16", "System.UInt16", "System.Int64", "System.UInt64", "System.Single", "System.Double", "System.Decimal", "System.DateTime" };

        protected CodeType codeType;
        private readonly Item parent;
        protected virtual CodeType CodeType => codeType;

        protected TypeInfo(CodeType codeType, Item parent)
        {
            this.codeType = codeType;
            this.parent = parent;
        }

        public Item Parent => parent;
        public virtual string Name => CodeType.Name;
        public virtual string FullName => CodeType.FullName;
        public virtual string Namespace => CodeType.Namespace.FullName;
        
        public bool IsGeneric => FullName.IndexOf("<", StringComparison.Ordinal) > -1 || IsNullable;
        public bool IsNullable => FullName.StartsWith("System.Nullable<") || FullName.EndsWith("?");
        public bool IsEnum => CodeType.Kind == vsCMElement.vsCMElementEnum;
        public bool IsEnumerable => FullName != "System.String" && (FullName.StartsWith("System.Collections.") || FullName == "System.Array");

        private Class baseClass;
        public Class BaseClass => baseClass ?? (baseClass = ClassInfo.FromCodeElements(CodeType.Bases, this).FirstOrDefault());

        private Attribute[] attributes;
        public ICollection<Attribute> Attributes => attributes ?? (attributes = AttributeInfo.FromCodeElements(CodeType.Attributes, this).ToArray());

        private Constant[] constants;
        public ICollection<Constant> Constants => constants ?? (constants = ConstantInfo.FromCodeElements(CodeType.Children, this).ToArray());

        private Field[] fields;
        public ICollection<Field> Fields => fields ?? (fields = FieldInfo.FromCodeElements(CodeType.Children, this).ToArray());

        private Type[] genericTypeArguments;
        public ICollection<Type> GenericTypeArguments => genericTypeArguments ?? (genericTypeArguments = LoadGenericTypeArguments().ToArray());

        private Interface[] interfaces;
        public ICollection<Interface> Interfaces => interfaces ?? (interfaces = InterfaceInfo.FromCodeElements(CodeType.Bases, this).ToArray());

        private Method[] methods;
        public ICollection<Method> Methods => methods ?? (methods = MethodInfo.FromCodeElements(CodeType.Children, this).ToArray());

        private Property[] properties;
        public ICollection<Property> Properties => properties ?? (properties = PropertyInfo.FromCodeElements(CodeType.Children, this).ToArray());

        private Class[] nestedClasses;
        public ICollection<Class> NestedClasses => nestedClasses ?? (nestedClasses = ClassInfo.FromCodeElements(CodeType.Members, this).ToArray());

        private Enum[] nestedEnums;
        public ICollection<Enum> NestedEnums => nestedEnums ?? (nestedEnums = EnumInfo.FromCodeElements(CodeType.Members, this).ToArray());

        private Interface[] nestedInterfaces;
        public ICollection<Interface> NestedInterfaces => nestedInterfaces ?? (nestedInterfaces = InterfaceInfo.FromCodeElements(CodeType.Members, this).ToArray());


        private IEnumerable<Type> LoadGenericTypeArguments()
        {
            if (IsGeneric == false) return new Type[0];
            if (FullName.EndsWith("?")) return new[] { new LazyTypeInfo(FullName.TrimEnd('?'), this) };

            return GenericTypeInfo.ExtractGenericTypeNames(FullName).Select(fullName =>
            {
                if (fullName.EndsWith("[]"))
                    fullName = $"System.Collections.Generic.ICollection<{fullName.TrimEnd('[', ']')}>";

                return new LazyTypeInfo(fullName, this);
            });
        }

        public bool IsPrimitive
        {
            get
            {
                var fullName = FullName;

                if (IsNullable)
                {
                    fullName = GenericTypeArguments.First().FullName;
                }
                else if (IsEnumerable)
                {
                    var innerType = GenericTypeArguments.FirstOrDefault();
                    if (innerType != null)
                    {
                        fullName = IsNullable ? innerType.GenericTypeArguments.First().FullName : innerType.FullName;
                    }
                    else
                    {
                        return false;
                    }
                }
                
                return IsEnum || primitiveTypes.Any(t => t == fullName);
            }
        }

        private static Type GetType(dynamic element, Item parent)
        {
            //try
            //{
            var isGenericTypeArgument = element.Type.TypeKind == (int)vsCMTypeRef.vsCMTypeRefOther && element.Type.AsFullName.Split('.').Length == 1;
            if (isGenericTypeArgument)
            {
                return new GenericTypeInfo(element.Type.AsFullName, parent);
            }

            var isArray = element.Type.TypeKind == (int)vsCMTypeRef.vsCMTypeRefArray;
            if (isArray)
            {
                var underlyingType = element.Type.ElementType.AsFullName;
                var collectionFormat = "System.Collections.Generic.ICollection<{0}>";

                return new LazyTypeInfo(string.Format(collectionFormat, underlyingType), parent);
            }

            return new TypeInfo(element.Type.CodeType, parent);
            //}
            //catch (NotImplementedException)
            //{
            //    return new LazyTypeInfo<T>(fullName, parent);
            //}
        }

        public static Type FromCodeElement(CodeVariable2 codeVariable, Item parent)
        {
            return GetType(codeVariable, parent);
        }

        public static Type FromCodeElement(CodeFunction2 codeVariable, Item parent)
        {
            return GetType(codeVariable, parent);
        }

        public static Type FromCodeElement(CodeProperty2 codeVariable, Item parent)
        {
            return GetType(codeVariable, parent);
        }

        public static Type FromCodeElement(CodeParameter2 codeVariable, Item parent)
        {
            return GetType(codeVariable, parent);
        }
    }
}