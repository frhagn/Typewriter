using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Typewriter.CodeModel.Collections;

namespace Typewriter.CodeModel.CodeDom
{
    public class CodeDomFile : File
    {
        private readonly ProjectItem projectItem;
        private readonly TypeFactory typeFactory;

        public CodeDomFile(ProjectItem projectItem)
        {
            this.projectItem = projectItem;
            this.typeFactory = new TypeFactory(projectItem);
        }

        public string Name => projectItem.Name;
        public string FullName => projectItem.FileNames[1];

        private IEnumerable<CodeNamespace> Namespaces => projectItem.FileCodeModel.CodeElements.OfType<CodeNamespace>();

        private ClassCollection classes;
        public ClassCollection Classes => classes ?? (classes = new ClassCollectionImpl(Namespaces.SelectMany(n => CodeDomClass.FromCodeElements(n.Members, this))));

        private EnumCollection enums;
        public EnumCollection Enums => enums ?? (enums = new EnumCollectionImpl(Namespaces.SelectMany(n => CodeDomEnum.FromCodeElements(n.Members, this))));

        private InterfaceCollection interfaces;
        public InterfaceCollection Interfaces => interfaces ?? (interfaces = new InterfaceCollectionImpl(Namespaces.SelectMany(n => CodeDomInterface.FromCodeElements(n.Members, this))));

        internal CodeType GetType(string fullName)
        {
            return typeFactory.GetType(fullName);
        }
    }
}