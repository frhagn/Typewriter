using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.CodeDom
{
    public class CodeDomClassMetadata : IClassMetadata
    {
        private readonly CodeClass2 codeClass;
        private readonly CodeDomFileMetadata file;

        private CodeDomClassMetadata(CodeClass2 codeClass, CodeDomFileMetadata file)
        {
            this.codeClass = codeClass;
            this.file = file;
        }

        public string DocComment => codeClass.DocComment;
        public string Name => codeClass.Name;
        public string FullName => codeClass.FullName;
        public string Namespace => GetNamespace();
        public ITypeMetadata Type => new LazyCodeDomTypeMetadata(codeClass.FullName, false, false, file);
        public bool IsAbstract => codeClass.IsAbstract;
        public bool IsGeneric => codeClass.IsGeneric;
        public IClassMetadata BaseClass => CodeDomClassMetadata.FromCodeElements(codeClass.Bases, file).FirstOrDefault();
        public IClassMetadata ContainingClass => CodeDomClassMetadata.FromCodeClass(codeClass.Parent as CodeClass2, file);
        public IEnumerable<IAttributeMetadata> Attributes => CodeDomAttributeMetadata.FromCodeElements(codeClass.Attributes);
        public IEnumerable<IConstantMetadata> Constants => CodeDomConstantMetadata.FromCodeElements(codeClass.Children, file);
        public IEnumerable<IDelegateMetadata> Delegates => CodeDomDelegateMetadata.FromCodeElements(codeClass.Children, file);
        public IEnumerable<IEventMetadata> Events => CodeDomEventMetadata.FromCodeElements(codeClass.Children, file);
        public IEnumerable<IFieldMetadata> Fields => CodeDomFieldMetadata.FromCodeElements(codeClass.Children, file);
        public IEnumerable<ITypeParameterMetadata> TypeParameters => CodeDomTypeParameterMetadata.FromFullName(codeClass.FullName);
        public IEnumerable<ITypeMetadata> TypeArguments => CodeDomTypeMetadata.LoadGenericTypeArguments(IsGeneric, FullName, file);
        public IEnumerable<IInterfaceMetadata> Interfaces => CodeDomInterfaceMetadata.FromCodeElements(codeClass.ImplementedInterfaces, file);
        public IEnumerable<IMethodMetadata> Methods => CodeDomMethodMetadata.FromCodeElements(codeClass.Children, file);
        public IEnumerable<IPropertyMetadata> Properties => CodeDomPropertyMetadata.FromCodeElements(codeClass.Children, file);
        public IEnumerable<IClassMetadata> NestedClasses => CodeDomClassMetadata.FromCodeElements(codeClass.Members, file);
        public IEnumerable<IEnumMetadata> NestedEnums => CodeDomEnumMetadata.FromCodeElements(codeClass.Members, file);
        public IEnumerable<IInterfaceMetadata> NestedInterfaces => CodeDomInterfaceMetadata.FromCodeElements(codeClass.Members, file);

        private string GetNamespace()
        {
            var parent = codeClass.Parent as CodeClass2;
            return parent != null ? parent.FullName : (codeClass.Namespace?.FullName ?? string.Empty);
        }

        internal static IEnumerable<IClassMetadata> FromCodeElements(CodeElements codeElements, CodeDomFileMetadata file)
        {
            return codeElements.OfType<CodeClass2>().Where(c => c.Access == vsCMAccess.vsCMAccessPublic && c.FullName != "System.Object").Select(c => new CodeDomClassMetadata(c, file));
        }

        internal static IClassMetadata FromCodeClass(CodeClass2 codeClass, CodeDomFileMetadata file)
        {
            return codeClass == null || codeClass.Access != vsCMAccess.vsCMAccessPublic || codeClass.FullName == "System.Object" ? null : new CodeDomClassMetadata(codeClass, file);
        }
    }
}