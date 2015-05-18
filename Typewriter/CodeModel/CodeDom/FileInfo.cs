using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.VisualStudio;

namespace Typewriter.CodeModel.CodeDom
{
    public class FileInfo : File
    {
        private static readonly object locker = new object();

        private readonly ProjectItem projectItem;
        private readonly IDictionary<string, CodeType> typeCache;

        private CodeNamespace currentNamespace;

        public FileInfo(ProjectItem projectItem)
        {
            this.projectItem = projectItem;
            this.typeCache = new Dictionary<string, CodeType>();

            var x = this.Classes;
        }

        public string Name
        {
            get { return projectItem.Name; }
        }

        public string FullName
        {
            get { return projectItem.FileNames[1]; }
        }

        private Class[] classes;
        public ICollection<Class> Classes
        {
            get 
            {
                if (classes == null)
                {
                    classes = GetNamespaces().SelectMany(n => Iterator<CodeClass2>.Select(() => n.Members, c => c.Access == vsCMAccess.vsCMAccessPublic, c => (Class)new ClassInfo(c, this, this))).ToArray();
                }
                return classes;
            }
        }

        private Enum[] enums;
        public ICollection<Enum> Enums
        {
            get
            {
                if (enums == null)
                {
                    enums = GetNamespaces().SelectMany(n => Iterator<CodeEnum>.Select(() => n.Members, e => e.Access == vsCMAccess.vsCMAccessPublic, e => (Enum)new EnumInfo(e, this, this))).ToArray();
                }
                return enums;
            }
        }

        private Interface[] interfaces;
        public ICollection<Interface> Interfaces
        {
            get
            {
                if (interfaces == null)
                {
                    interfaces = GetNamespaces().SelectMany(n => Iterator<CodeInterface2>.Select(() => n.Members, i => i.Access == vsCMAccess.vsCMAccessPublic, i => (Interface)new InterfaceInfo(i, this, this))).ToArray();
                }
                return interfaces;
            }
        }

        public CodeType GetType(string fullName)
        {
            lock (locker)
            {
                if (typeCache.ContainsKey(fullName)) return typeCache[fullName];

                var stopwatch = Stopwatch.StartNew();

                var undoContext = projectItem.DTE.UndoContext;
                var undo = undoContext.IsOpen == false; // && projectItem.DTE.Documents.Cast<Document>().Any(document => document.ProjectItem == projectItem);

                CodeType typeInfo;

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
                        typeInfo = variable.Type.CodeType;
                    }
                        //catch // (NotImplementedException)
                        //{
                        //    typeInfo = new ObjectTypeInfo();
                        //}
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
                Log.Debug("GetType({0}) completed in {1} ms", fullName, stopwatch.ElapsedMilliseconds);

                return typeInfo;
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