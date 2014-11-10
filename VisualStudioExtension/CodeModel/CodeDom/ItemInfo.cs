using System;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class ItemInfo : IItemInfo
    {
        private readonly dynamic element;
        protected readonly FileInfo file;

        public ItemInfo(dynamic element, FileInfo file)
        {
            this.element = element;
            this.file = file;
        }

        public string Name
        {
            get { return element.Name; }
        }

        public string FullName
        {
            get { return element.FullName; }
        }

        public IEnumerable<IAttributeInfo> Attributes
        {
            get { return Iterator<CodeAttribute2>.Select(() => element.Children, a => new AttributeInfo(a, file)); }
        }

        public IEnumerable<IConstantInfo> Constants
        {
            get { return Iterator<CodeVariable2>.Select(() => element.Children, v => v.IsConstant, f => new ConstantInfo(f, file)); }
        }

        public IEnumerable<IFieldInfo> Fields
        {
            get { return Iterator<CodeVariable2>.Select(() => element.Children, v => v.IsConstant == false, f => new FieldInfo(f, file)); }
        }

        public IEnumerable<IInterfaceInfo> Interfaces
        {
            get
            {
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

                return Iterator<CodeInterface2>.Select(func, i => new InterfaceInfo(i, file));
            }
        }

        public IEnumerable<IMethodInfo> Methods
        {
            get { return Iterator<CodeFunction2>.Select(() => element.Children, m => m.FunctionKind != vsCMFunction.vsCMFunctionConstructor, f => new MethodInfo(f, file)); }
        }

        public IEnumerable<IParameterInfo> Parameters
        {
            get { return Iterator<CodeParameter2>.Select(() => element.Children, p => new ParameterInfo(p, file)); }
        }

        public IEnumerable<IPropertyInfo> Properties
        {
            get { return Iterator<CodeProperty2>.Select(() => element.Children, p => new PropertyInfo(p, file)); }
        }

        public ITypeInfo Type
        {
            get
            {
                try
                {
                    if (element.Type.TypeKind == (int)vsCMTypeRef.vsCMTypeRefArray)
                    {
                        // Fulhack för Array-type som är definierade i kod
                        return file.GetType(string.Format("System.Collections.Generic.IEnumerable<{0}>", element.Type.ElementType.AsFullName));
                    }

                    return new TypeInfo(element.Type.CodeType, file);
                }
                catch (NotImplementedException)
                {
                    return new ObjectTypeInfo();
                }
            }
        }
    }
}