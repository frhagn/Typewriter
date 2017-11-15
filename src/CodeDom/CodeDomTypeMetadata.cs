using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.CodeDom
{
    public class CodeDomTypeMetadata : ITypeMetadata
    {
        protected CodeType codeType;
        private readonly bool isNullable;
        private readonly bool isTask;
        private readonly CodeDomFileMetadata file;
        protected virtual CodeType CodeType => codeType;

        protected CodeDomTypeMetadata(CodeType codeType, bool isNullable, bool isTask, CodeDomFileMetadata file)
        {
            this.codeType = codeType;
            this.isNullable = isNullable;
            this.isTask = isTask;
            this.file = file;
        }

        public string DocComment => CodeType.DocComment;
        public virtual string Name => GetName(CodeType.Name);
        public virtual string FullName => GetFullName(CodeType.FullName);
        public virtual string Namespace => GetNamespace();
        public ITypeMetadata Type => this;

        public bool IsAbstract => (codeType as CodeClass2)?.IsAbstract ?? false;
        public bool IsEnum => CodeType.Kind == vsCMElement.vsCMElementEnum;
        public bool IsEnumerable => IsCollection(FullName);
        public bool IsGeneric => FullName.IndexOf("<", StringComparison.Ordinal) > -1 && IsNullable == false;
        public bool IsNullable => isNullable;
        public bool IsTask => isTask;
        public bool IsDefined => CodeType.InfoLocation == vsCMInfoLocation.vsCMInfoLocationProject;
        public bool IsValueTuple => false;
        public IEnumerable<IFieldMetadata> TupleElements => new IFieldMetadata[0];

        public IEnumerable<IAttributeMetadata> Attributes => CodeDomAttributeMetadata.FromCodeElements(CodeType.Attributes);
        public IEnumerable<ITypeMetadata> TypeArguments => LoadGenericTypeArguments(IsGeneric, FullName, file);
        public IEnumerable<ITypeParameterMetadata> TypeParameters => CodeDomTypeParameterMetadata.FromFullName(FullName);

        public IClassMetadata BaseClass => CodeDomClassMetadata.FromCodeElements(CodeType.Bases, file).FirstOrDefault();
        public IClassMetadata ContainingClass => CodeDomClassMetadata.FromCodeClass(CodeType.Parent as CodeClass2, file);
        public IEnumerable<IConstantMetadata> Constants => CodeDomConstantMetadata.FromCodeElements(CodeType.Children, file);
        public IEnumerable<IDelegateMetadata> Delegates => CodeDomDelegateMetadata.FromCodeElements(CodeType.Children, file);
        public IEnumerable<IEventMetadata> Events => CodeDomEventMetadata.FromCodeElements(CodeType.Children, file);
        public IEnumerable<IFieldMetadata> Fields => CodeDomFieldMetadata.FromCodeElements(CodeType.Children, file);
        public IEnumerable<IInterfaceMetadata> Interfaces => CodeDomInterfaceMetadata.FromCodeElements(CodeType.Bases, file);
        public IEnumerable<IMethodMetadata> Methods => CodeDomMethodMetadata.FromCodeElements(CodeType.Children, file);
        public IEnumerable<IPropertyMetadata> Properties => CodeDomPropertyMetadata.FromCodeElements(CodeType.Children, file);
        public IEnumerable<IClassMetadata> NestedClasses => CodeDomClassMetadata.FromCodeElements(CodeType.Members, file);
        public IEnumerable<IEnumMetadata> NestedEnums => CodeDomEnumMetadata.FromCodeElements(CodeType.Members, file);
        public IEnumerable<IInterfaceMetadata> NestedInterfaces => CodeDomInterfaceMetadata.FromCodeElements(CodeType.Members, file);


        private string GetNamespace()
        {
            var parent = CodeType.Parent as CodeClass2;
            return parent != null ? parent.FullName : (CodeType.Namespace?.FullName ?? string.Empty);
        }

        protected string GetName(string name)
        {
            return name + (IsNullable ? "?" : string.Empty);
        }

        protected string GetFullName(string fullName)
        {
            return fullName + (IsNullable ? "?" : string.Empty);
        }

        public static IEnumerable<ITypeMetadata> LoadGenericTypeArguments(bool isGeneric, string typeFullName, CodeDomFileMetadata file)
        {
            if (isGeneric == false) return new ITypeMetadata[0];

            return LazyCodeDomTypeMetadata.ExtractGenericTypeNames(typeFullName).Select(fullName =>
            {
                if (fullName.EndsWith("[]"))
                    fullName = $"System.Collections.Generic.ICollection<{fullName.TrimEnd('[', ']')}>";

                var isNullable = fullName.EndsWith("?") || fullName.StartsWith("System.Nullable<");
                if (isNullable)
                {
                    fullName = fullName.EndsWith("?") ? fullName.TrimEnd('?') : fullName.Substring(16, fullName.Length - 17);
                    return new LazyCodeDomTypeMetadata(fullName, true, false, file);
                }

                var isTask = fullName.StartsWith("System.Threading.Tasks.Task");
                if (isTask)
                {
                    fullName = fullName.Contains("<") ? fullName.Substring(28, fullName.Length - 29) : "System.Void";

                    isNullable = fullName.EndsWith("?") || fullName.StartsWith("System.Nullable<");
                    if (isNullable)
                    {
                        fullName = fullName.EndsWith("?") ? fullName.TrimEnd('?') : fullName.Substring(16, fullName.Length - 17);
                        return new LazyCodeDomTypeMetadata(fullName, true, true, file);
                    }

                    return new LazyCodeDomTypeMetadata(fullName, false, true, file);
                }

                return new LazyCodeDomTypeMetadata(fullName, false, false, file);
            });
        }

        private static ITypeMetadata GetType(dynamic element, CodeDomFileMetadata file)
        {
            var isGenericTypeArgument = element.Type.TypeKind == (int)vsCMTypeRef.vsCMTypeRefOther && element.Type.AsFullName.Split('.').Length == 1;
            if (isGenericTypeArgument)
            {
                return new GenericTypeMetadata(element.Type.AsFullName);
            }

            var isArray = element.Type.TypeKind == (int)vsCMTypeRef.vsCMTypeRefArray;
            if (isArray)
            {
                var name = element.Type.ElementType.AsFullName;
                return new LazyCodeDomTypeMetadata($"System.Collections.Generic.ICollection<{name}>", false, false, file);
            }

            CodeType codeType = element.Type.CodeType;

            var isNullable = codeType.FullName.EndsWith("?") || codeType.FullName.StartsWith("System.Nullable<");
            if (isNullable)
            {
                var name = codeType.FullName;
                name = name.EndsWith("?") ? name.TrimEnd('?') : name.Substring(16, name.Length - 17);

                return new LazyCodeDomTypeMetadata(name, true, false, file);
            }

            var isTask = codeType.FullName.StartsWith("System.Threading.Tasks.Task");
            if (isTask)
            {
                var name = codeType.FullName;
                name = name.Contains("<") ? name.Substring(28, name.Length - 29) : "System.Void";

                isNullable = name.EndsWith("?") || name.StartsWith("System.Nullable<");
                if (isNullable)
                {
                    name = name.EndsWith("?") ? name.TrimEnd('?') : name.Substring(16, name.Length - 17);
                    return new LazyCodeDomTypeMetadata(name, true, true, file);
                }

                return new LazyCodeDomTypeMetadata(name, false, true, file);
            }

            return new CodeDomTypeMetadata(codeType, false, false, file);
        }

        private static bool IsCollection(string fullName)
        {
            if (fullName == "System.Array") return true;
            if (fullName.StartsWith("System.Collections.") == false) return false;

            fullName = fullName.Split('<')[0];

            if (fullName.Contains("Comparer")) return false;
            if (fullName.Contains("Enumerator")) return false;
            if (fullName.Contains("Provider")) return false;
            if (fullName.Contains("Partitioner")) return false;
            if (fullName.Contains("Structural")) return false;
            if (fullName.Contains("KeyNotFoundException")) return false;
            if (fullName.Contains("KeyValuePair")) return false;

            return true;
        }

        public static ITypeMetadata FromCodeElement(CodeVariable2 codeVariable, CodeDomFileMetadata file)
        {
            return GetType(codeVariable, file);
        }

        public static ITypeMetadata FromCodeElement(CodeFunction2 codeVariable, CodeDomFileMetadata file)
        {
            return GetType(codeVariable, file);
        }

        public static ITypeMetadata FromCodeElement(CodeDelegate2 codeVariable, CodeDomFileMetadata file)
        {
            return GetType(codeVariable, file);
        }

        public static ITypeMetadata FromCodeElement(CodeEvent codeVariable, CodeDomFileMetadata file)
        {
            return GetType(codeVariable, file);
        }

        public static ITypeMetadata FromCodeElement(CodeProperty2 codeVariable, CodeDomFileMetadata file)
        {
            return GetType(codeVariable, file);
        }

        public static ITypeMetadata FromCodeElement(CodeParameter2 codeVariable, CodeDomFileMetadata file)
        {
            return GetType(codeVariable, file);
        }
    }
}