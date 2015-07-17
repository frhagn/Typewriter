using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.CodeModel.Collections;

namespace Typewriter.CodeModel.CodeDom
{
    public class CodeDomType : Type
    {
        private static readonly string[] primitiveTypes = { "System.Boolean", "System.Char", "System.String", "System.Byte", "System.SByte", "System.Int32", "System.UInt32", "System.Int16", "System.UInt16", "System.Int64", "System.UInt64", "System.Single", "System.Double", "System.Decimal", "System.DateTime" };

        protected CodeType codeType;
        private readonly Item parent;
        protected virtual CodeType CodeType => codeType;

        protected CodeDomType(CodeType codeType, Item parent)
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
        public bool IsEnumerable => FullName != "System.String" && (FullName == "System.Array" || IsCollection(FullName));

        private Class baseClass;
        public Class BaseClass => baseClass ?? (baseClass = CodeDomClass.FromCodeElements(CodeType.Bases, this).FirstOrDefault());

        private Class containingClass;
        public Class ContainingClass => containingClass ?? (containingClass = CodeDomClass.FromCodeClass(CodeType.Parent as CodeClass2, this));

        private AttributeCollection attributes;
        public AttributeCollection Attributes => attributes ?? (attributes = new AttributeCollectionImpl(CodeDomAttribute.FromCodeElements(CodeType.Attributes, this)));

        private ConstantCollection constants;
        public ConstantCollection Constants => constants ?? (constants = new ConstantCollectionImpl(CodeDomConstant.FromCodeElements(CodeType.Children, this)));

        private FieldCollection fields;
        public FieldCollection Fields => fields ?? (fields = new FieldCollectionImpl(CodeDomField.FromCodeElements(CodeType.Children, this)));

        private TypeCollection genericTypeArguments;
        public TypeCollection GenericTypeArguments => genericTypeArguments ?? (genericTypeArguments = new TypeCollectionImpl(LoadGenericTypeArguments()));

        private InterfaceCollection interfaces;
        public InterfaceCollection Interfaces => interfaces ?? (interfaces = new InterfaceCollectionImpl(CodeDomInterface.FromCodeElements(CodeType.Bases, this)));

        private MethodCollection methods;
        public MethodCollection Methods => methods ?? (methods = new MethodCollectionImpl(CodeDomMethod.FromCodeElements(CodeType.Children, this)));

        private PropertyCollection properties;
        public PropertyCollection Properties => properties ?? (properties = new PropertyCollectionImpl(CodeDomProperty.FromCodeElements(CodeType.Children, this)));

        private ClassCollection nestedClasses;
        public ClassCollection NestedClasses => nestedClasses ?? (nestedClasses = new ClassCollectionImpl(CodeDomClass.FromCodeElements(CodeType.Members, this)));

        private EnumCollection nestedEnums;
        public EnumCollection NestedEnums => nestedEnums ?? (nestedEnums = new EnumCollectionImpl(CodeDomEnum.FromCodeElements(CodeType.Members, this)));

        private InterfaceCollection nestedInterfaces;
        public InterfaceCollection NestedInterfaces => nestedInterfaces ?? (nestedInterfaces = new InterfaceCollectionImpl(CodeDomInterface.FromCodeElements(CodeType.Members, this)));
        
        private IEnumerable<Type> LoadGenericTypeArguments()
        {
            if (IsGeneric == false) return new Type[0];
            if (FullName.EndsWith("?")) return new[] { new LazyCodeDomType(FullName.TrimEnd('?'), this) };

            return GenericTypeInfo.ExtractGenericTypeNames(FullName).Select(fullName =>
            {
                if (fullName.EndsWith("[]"))
                    fullName = $"System.Collections.Generic.ICollection<{fullName.TrimEnd('[', ']')}>";

                return new LazyCodeDomType(fullName, this);
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

                return new LazyCodeDomType(string.Format(collectionFormat, underlyingType), parent);
            }

            return new CodeDomType(element.Type.CodeType, parent);
            //}
            //catch (NotImplementedException)
            //{
            //    return new LazyCodeDomType<T>(fullName, parent);
            //}
        }

        private static bool IsCollection(string fullName)
        {
            if (fullName.StartsWith("System.Collections.") == false) return false;

            if (fullName.Contains("Comparer")) return false;
            if (fullName.Contains("Enumerator")) return false;
            if (fullName.Contains("Provider")) return false;
            if (fullName.Contains("Partitioner")) return false;
            if (fullName.Contains("Structural")) return false;
            if (fullName.Contains("KeyNotFoundException")) return false;
            if (fullName.Contains("KeyValuePair")) return false;

            return true;
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