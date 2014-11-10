using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class FileInfo : IFileInfo
    {
        private readonly IDictionary<string, ITypeInfo> typeCache;
        private readonly ProjectItem projectItem;

        private CodeNamespace currentNamespace;

        public FileInfo(ProjectItem projectItem)
        {
            this.typeCache = new Dictionary<string, ITypeInfo>();
            this.projectItem = projectItem;
        }

        public string Name
        {
            get { return projectItem.Name; }
        }

        public string FullName
        {
            get { return projectItem.FileNames[1]; }
        }

        public IEnumerable<IClassInfo> Classes
        {
            get { return GetNamespaces().SelectMany(n => Iterator<CodeClass2>.Select(() => n.Children, c => new ClassInfo(c, this))); }
        }

        public IEnumerable<IEnumInfo> Enums
        {
            get { return GetNamespaces().SelectMany(n => Iterator<CodeEnum>.Select(() => n.Children, e => new EnumInfo(e, this))); }
        }

        public IEnumerable<IInterfaceInfo> Interfaces
        {
            get { return GetNamespaces().SelectMany(n => Iterator<CodeInterface2>.Select(() => n.Children, i => new InterfaceInfo(i, this))); }
        }

        public ITypeInfo GetType(string fullName)
        {
            if (typeCache.ContainsKey(fullName)) return typeCache[fullName];

            var undoContext = projectItem.DTE.UndoContext;
            var undo = undoContext.IsOpen == false;// && projectItem.DTE.Documents.Cast<Document>().Any(document => document.ProjectItem == projectItem);

            try
            {
                if (undo) undoContext.Open("GetType");

                ITypeInfo typeInfo;
                var classAdded = false;
                var name = Guid.NewGuid().ToString("N");

                var codeClass = new Iterator<CodeClass>(currentNamespace.Children).FirstOrDefault();

                if (codeClass == null)
                {
                    codeClass = currentNamespace.AddClass("c" + name, -1, null, null, vsCMAccess.vsCMAccessPublic);
                    classAdded = true;
                }
                var variable = codeClass.AddVariable("v" + name, fullName, -1, vsCMAccess.vsCMAccessPublic, null);

                try
                {
                    typeInfo = new TypeInfo(variable.Type.CodeType, this);
                }
                catch // (NotImplementedException)
                {
                    typeInfo = new ObjectTypeInfo();
                }
                finally
                {
                    if (undo == false)
                    {
                        if (classAdded)
                            currentNamespace.Remove(codeClass);
                        else
                            codeClass.RemoveMember(variable);
                    }
                }

                typeCache.Add(fullName, typeInfo);
                return typeInfo;
            }
            finally
            {
                if (undo) undoContext.SetAborted();
            }
        }

        private IEnumerable<CodeNamespace> GetNamespaces()
        {
            return new Iterator<CodeNamespace>(projectItem.FileCodeModel.CodeElements).Select(ns =>
            {
                this.currentNamespace = ns;
                return ns;
            });
        }
    }
}