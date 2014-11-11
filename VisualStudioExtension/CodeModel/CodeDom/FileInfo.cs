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
        private readonly ILog log;
        private readonly ProjectItem projectItem;
        private readonly IDictionary<string, ITypeInfo> typeCache;

        private CodeNamespace currentNamespace;

        public FileInfo(ILog log, ProjectItem projectItem)
        {
            this.log = log;
            this.projectItem = projectItem;
            this.typeCache = new Dictionary<string, ITypeInfo>();
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

            var stopwatch = Stopwatch.StartNew();

            var undoContext = projectItem.DTE.UndoContext;
            var undo = undoContext.IsOpen == false;// && projectItem.DTE.Documents.Cast<Document>().Any(document => document.ProjectItem == projectItem);

            ITypeInfo typeInfo;

            try
            {
                if (undo) undoContext.Open("GetType");

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
            }
            finally
            {
                if (undo) undoContext.SetAborted();
            }

            stopwatch.Stop();
            log.Debug("GetType({0}) completed in {1} ms", fullName, stopwatch.ElapsedMilliseconds);

            return typeInfo;
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