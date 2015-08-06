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
        private readonly CodeDomFileMetadata file;
        protected virtual CodeType CodeType => codeType;

        protected CodeDomTypeMetadata(CodeType codeType, bool isNullable, CodeDomFileMetadata file)
        {
            this.codeType = codeType;
            this.isNullable = isNullable;
            this.file = file;
        }

        public virtual string Name => GetName(CodeType.Name);
        public virtual string FullName => GetFullName(CodeType.FullName);
        public virtual string Namespace => GetNamespace();

        public bool IsGeneric => FullName.IndexOf("<", StringComparison.Ordinal) > -1 && IsNullable == false;
        public bool IsNullable => isNullable;
        public bool IsEnum => CodeType.Kind == vsCMElement.vsCMElementEnum;
        public bool IsEnumerable => IsCollection(FullName);

        public IEnumerable<IAttributeMetadata> Attributes => CodeDomAttributeMetadata.FromCodeElements(CodeType.Attributes);
        public IEnumerable<ITypeMetadata> TypeArguments => LoadGenericTypeArguments();
        
        private string GetNamespace()
        {
            var parent = CodeType.Parent as CodeClass2;
            return parent != null ? parent.FullName : CodeType.Namespace.FullName;
        }

        protected string GetName(string name)
        {
            return name + (IsNullable ? "?" : string.Empty);
        }

        protected string GetFullName(string fullName)
        {
            return fullName + (IsNullable ? "?" : string.Empty);
        }

        private IEnumerable<ITypeMetadata> LoadGenericTypeArguments()
        {
            if (IsGeneric == false) return new ITypeMetadata[0];

            return LazyCodeDomTypeMetadata.ExtractGenericTypeNames(FullName).Select(fullName =>
            {
                if (fullName.EndsWith("[]"))
                    fullName = $"System.Collections.Generic.ICollection<{fullName.TrimEnd('[', ']')}>";

                var nullable = fullName.EndsWith("?") || fullName.StartsWith("System.Nullable<");
                if (nullable)
                {
                    fullName = fullName.EndsWith("?") ? fullName.TrimEnd('?') : fullName.Substring(16, fullName.Length - 17);
                    return new LazyCodeDomTypeMetadata(fullName, true, file);
                }

                return new LazyCodeDomTypeMetadata(fullName, false, file);
            });
        }

        private static ITypeMetadata GetType(dynamic element, CodeDomFileMetadata file)
        {
            //try
            //{
            var isGenericTypeArgument = element.Type.TypeKind == (int)vsCMTypeRef.vsCMTypeRefOther && element.Type.AsFullName.Split('.').Length == 1;
            if (isGenericTypeArgument)
            {
                return new GenericTypeMetadata(element.Type.AsFullName);
            }

            var isArray = element.Type.TypeKind == (int)vsCMTypeRef.vsCMTypeRefArray;
            if (isArray)
            {
                var name = element.Type.ElementType.AsFullName;
                return new LazyCodeDomTypeMetadata($"System.Collections.Generic.ICollection<{name}>", false, file);
            }

            CodeType codeType = element.Type.CodeType;
            
            var isNullable = codeType.FullName.EndsWith("?") || codeType.FullName.StartsWith("System.Nullable<");
            if (isNullable)
            {
                var name = codeType.FullName;
                name = name.EndsWith("?") ? name.TrimEnd('?') : name.Substring(16, name.Length - 17);

                return new LazyCodeDomTypeMetadata(name, true, file);
            }

            return new CodeDomTypeMetadata(codeType, false, file);
            //}
            //catch (NotImplementedException)
            //{
            //    return new LazyCodeDomType<T>(fullName, parent);
            //}
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