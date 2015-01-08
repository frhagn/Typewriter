using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public abstract class ItemInfo : IItemInfo
    {
        protected dynamic element;
        protected readonly FileInfo file;

        protected ItemInfo(dynamic element, object parent, FileInfo file)
        {
            this.element = element;
            this.file = file;
            this.Parent = parent;
        }

        public object Parent { get; private set; }

        public virtual string Name
        {
            get { return element.Name; }
        }

        public virtual string FullName
        {
            get { return element.FullName; }
        }

        public virtual string Namespace
        {
            get { return element.Namespace.FullName; }
        }

        private IAttributeInfo[] attributes;
        public virtual ICollection<IAttributeInfo> Attributes
        {
            get
            {
                if (attributes == null)
                {
                    Load();
                    attributes = Iterator<CodeAttribute2>.Select(() => element.Children, a => (IAttributeInfo)new AttributeInfo(a, this, file)).ToArray();
                }
                return attributes;
            }
        }

        private IConstantInfo[] constants;
        public virtual ICollection<IConstantInfo> Constants
        {
            get
            {
                if (constants == null)
                {
                    Load();
                    constants = Iterator<CodeVariable2>.Select(() => element.Children, v => v.IsConstant, f => (IConstantInfo)new ConstantInfo(f, this, file)).ToArray();
                }
                return constants;
            }
        }

        private IFieldInfo[] fields;
        public virtual ICollection<IFieldInfo> Fields
        {
            get
            {
                if (fields == null)
                {
                    Load();
                    fields = Iterator<CodeVariable2>.Select(() => element.Children, v => v.IsConstant == false, f => (IFieldInfo)new FieldInfo(f, this, file)).ToArray();
                }
                return fields;
            }
        }

        private IInterfaceInfo[] interfaces;
        public virtual ICollection<IInterfaceInfo> Interfaces
        {
            get
            {
                if (interfaces == null)
                {
                    Load();
                    Func<CodeElements> func = () =>
                    {
                        var elements = element.Children;

                        var codeType = element as CodeType;
                        if (codeType != null) elements = codeType.Bases;

                        var codeClass = element as CodeClass2;
                        if (codeClass != null) elements = codeClass.ImplementedInterfaces;

                        var codeInterface = element as CodeInterface2;
                        if (codeInterface != null) elements = codeInterface.Bases;

                        return elements;
                    };

                    interfaces = Iterator<CodeInterface2>.Select(func, i => (IInterfaceInfo)new InterfaceInfo(i, this, file)).ToArray();
                }
                return interfaces;
            }
        }

        private IMethodInfo[] methods;
        public virtual ICollection<IMethodInfo> Methods
        {
            get
            {
                if (methods == null)
                {
                    Load();
                    methods = Iterator<CodeFunction2>.Select(() => element.Children, m => m.FunctionKind != vsCMFunction.vsCMFunctionConstructor, f => (IMethodInfo)new MethodInfo(f, this, file)).ToArray();
                }
                return methods;
            }
        }

        private IParameterInfo[] parameters;
        public virtual ICollection<IParameterInfo> Parameters
        {
            get
            {
                if (parameters == null)
                {
                    Load();
                    parameters = Iterator<CodeParameter2>.Select(() => element.Children, p => (IParameterInfo)new ParameterInfo(p, this, file)).ToArray();
                }
                return parameters;
            }
        }

        private IPropertyInfo[] properties;
        public virtual ICollection<IPropertyInfo> Properties
        {
            get
            {
                if (properties == null)
                {
                    Load();
                    properties = Iterator<CodeProperty2>.Select(() => element.Children, p => (IPropertyInfo)new PropertyInfo(p, this, file)).ToArray();
                }
                return properties;
            }
        }

        public virtual bool IsEnum
        {
            get { return this.Type.IsEnum; }
        }

        public virtual bool IsEnumerable
        {
            get { return ((TypeInfo)this.Type).IsEnumerable; }
        }

        public virtual bool IsGeneric
        {
            get { return ((TypeInfo)this.Type).IsGeneric; }
        }

        public virtual bool IsNullable
        {
            get { return ((TypeInfo)this.Type).IsNullable; }
        }

        public virtual bool IsPrimitive
        {
            get { return ((TypeInfo)this.Type).IsPrimitive; }
        }

        private ITypeInfo type;
        public virtual ITypeInfo Type
        {
            get
            {
                if (type == null)
                {
                    Load();
                    try
                    {
                        type = element.Type.TypeKind == (int)vsCMTypeRef.vsCMTypeRefArray ? 
                            new TypeInfo(string.Format("System.Collections.Generic.ICollection<{0}>", element.Type.ElementType.AsFullName), this, file) : 
                            new TypeInfo(element.Type.CodeType, this, file);
                    }
                    catch (NotImplementedException)
                    {
                        type = new TypeInfo(FullName, this, file);
                    }
                }
                return type;
            }
        }

        protected virtual void Load()
        {
        }
    }
}