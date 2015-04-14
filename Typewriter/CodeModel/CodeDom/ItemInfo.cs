﻿using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using System.Text;

namespace Typewriter.CodeModel.CodeDom
{
    public abstract class ItemInfo : Item
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
        public virtual string Name { get { return element.Name; } }
        public virtual string FullName { get { return element.FullName; } }
        public virtual string Namespace { get { return element.Namespace.FullName; } }

        public virtual bool IsEnum { get { return this.Type.IsEnum; } }
        public virtual bool IsEnumerable { get { return this.Type.IsEnumerable; } }
        public virtual bool IsGeneric { get { return this.Type.IsGeneric; } }
        public virtual bool IsNullable { get { return this.Type.IsNullable; } }
        public virtual bool IsPrimitive { get { return this.Type.IsPrimitive; } }

        private Attribute[] attributes;
        public virtual ICollection<Attribute> Attributes
        {
            get
            {
                if (attributes == null)
                {
                    Load();
                    attributes = Iterator<CodeAttribute2>.Select(() => element.Children, a => (Attribute)new AttributeInfo(a, this, file)).ToArray();
                }
                return attributes;
            }
        }

        private Constant[] constants;
        public virtual ICollection<Constant> Constants
        {
            get
            {
                if (constants == null)
                {
                    Load();
                    constants = Iterator<CodeVariable2>.Select(() => element.Children, v => v.IsConstant, f => (Constant)new ConstantInfo(f, this, file)).ToArray();
                }
                return constants;
            }
        }

        private Field[] fields;
        public virtual ICollection<Field> Fields
        {
            get
            {
                if (fields == null)
                {
                    Load();
                    fields = Iterator<CodeVariable2>.Select(() => element.Children, v => v.IsConstant == false, f => (Field)new FieldInfo(f, this, file)).ToArray();
                }
                return fields;
            }
        }

        private Interface[] interfaces;
        public virtual ICollection<Interface> Interfaces
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

                    interfaces = Iterator<CodeInterface2>.Select(func, i => (Interface)new InterfaceInfo(i, this, file)).ToArray();
                }
                return interfaces;
            }
        }

        private Method[] methods;
        public virtual ICollection<Method> Methods
        {
            get
            {
                if (methods == null)
                {
                    Load();
                    methods = Iterator<CodeFunction2>.Select(() => element.Children, m => m.FunctionKind != vsCMFunction.vsCMFunctionConstructor, f => (Method)new MethodInfo(f, this, file)).ToArray();
                }
                return methods;
            }
        }

        private Parameter[] parameters;
        public virtual ICollection<Parameter> Parameters
        {
            get
            {
                if (parameters == null)
                {
                    Load();
                    parameters = Iterator<CodeParameter2>.Select(() => element.Children, p => (Parameter)new ParameterInfo(p, this, file)).ToArray();
                }
                return parameters;
            }
        }

        private Property[] properties;
        public virtual ICollection<Property> Properties
        {
            get
            {
                if (properties == null)
                {
                    Load();
                    properties = Iterator<CodeProperty2>.Select(() => element.Children, p => (Property)new PropertyInfo(p, this, file)).ToArray();
                }
                return properties;
            }
        }
        
        private Type type;
        public virtual Type Type
        {
            get
            {
                if (type == null)
                {
                    Load();
                    
                    type = GetTypeInfo();
                }
                return type;
            }
        }

        protected virtual void Load()
        {
        }

        private TypeInfo GetTypeInfo()
        {
            try
            {
                var type = element.Type;
                var typeKind = element.Type.TypeKind;
                var isArray = typeKind == (int)vsCMTypeRef.vsCMTypeRefArray;
                var isGenericTypeArgument = typeKind == (int)vsCMTypeRef.vsCMTypeRefOther
                    && element.Type.AsFullName.Split('.').Length == 1;

                if (isGenericTypeArgument)
                {
                    return new TypeInfo(element.Type.AsFullName, this, file);
                }
                else if (isArray)
                {
                    var underlyingType = element.Type.ElementType.AsFullName;
                    var collectionFormat = "System.Collections.Generic.ICollection<{0}>";

                    return new TypeInfo(string.Format(collectionFormat, underlyingType), this, file);
                }
                else
                {
                    return new TypeInfo(element.Type.CodeType, this, file);
                }
            }
            catch (NotImplementedException)
            {
                return new TypeInfo(FullName, this, file);
            }
        }

        public IEnumerable<Type> GenericTypeArguments
        {
            get
            {
                if (IsGeneric == false) return new Type[0];
                if (IsNullable && FullName.EndsWith("?")) return new[] { new TypeInfo(FullName.TrimEnd('?'), this, file) };

                return ExtractGenericTypeNames(FullName).Select(n =>
                {
                    if (n.EndsWith("[]"))
                    {
                        n = string.Format("System.Collections.Generic.ICollection<{0}>", n);
                    }
                    return new TypeInfo(n, this, file);
                });
            }
        }

        private static IEnumerable<string> ExtractGenericTypeNames(string name)
        {
            var list = new List<string>();
            var start = name.IndexOf("<", StringComparison.Ordinal);
            var end = name.LastIndexOf(">", StringComparison.Ordinal) - (start + 1);

            if (start < 0)
            {
                return list;
            }

            var arguments = name.Substring(start + 1, end);

            var current = new StringBuilder();
            var level = 0;
            foreach (var character in arguments)
            {
                if (character == ',' && level == 0)
                {
                    list.Add(current.ToString());
                    current = new StringBuilder();
                }
                else
                {
                    if (character == '<')
                        level++;
                    else if (character == '>')
                        level--;

                    current.Append(character);
                }
            }

            if (current.Length > 0)
                list.Add(current.ToString());

            return list;
        }

    }
}