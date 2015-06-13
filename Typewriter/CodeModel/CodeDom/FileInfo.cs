using System.Collections.Generic;
using System.Linq;
using EnvDTE;

namespace Typewriter.CodeModel.CodeDom
{
    public class FileInfo : File
    {
        private readonly ProjectItem projectItem;
        private readonly TypeFactory typeFactory;

        public FileInfo(ProjectItem projectItem)
        {
            this.projectItem = projectItem;
            this.typeFactory = new TypeFactory(projectItem);
        }

        public string Name => projectItem.Name;
        public string FullName => projectItem.FileNames[1];

        private IEnumerable<CodeNamespace> Namespaces => projectItem.FileCodeModel.CodeElements.OfType<CodeNamespace>();

        private Class[] classes;
        public ICollection<Class> Classes => classes ?? (classes = Namespaces.SelectMany(n => ClassInfo.FromCodeElements(n.Members, this)).ToArray());

        private Enum[] enums;
        public ICollection<Enum> Enums => enums ?? (enums = Namespaces.SelectMany(n => EnumInfo.FromCodeElements(n.Members, this)).ToArray());

        private Interface[] interfaces;
        public ICollection<Interface> Interfaces => interfaces ?? (interfaces = Namespaces.SelectMany(n => InterfaceInfo.FromCodeElements(n.Members, this)).ToArray());

        internal CodeType GetType(string fullName)
        {
            return typeFactory.GetType(fullName);
        }
    }
}