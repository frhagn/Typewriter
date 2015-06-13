using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EnvDTE;
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
        }

        public string Name => projectItem.Name;
        public string FullName => projectItem.FileNames[1];

        private Class[] classes;
        public ICollection<Class> Classes => classes ?? (classes = GetNamespaces().SelectMany(n => ClassInfo.FromCodeElements(n.Members, this)).ToArray());

        private Enum[] enums;
        public ICollection<Enum> Enums => enums ?? (enums = GetNamespaces().SelectMany(n => EnumInfo.FromCodeElements(n.Members, this)).ToArray());
        
        private Interface[] interfaces;
        public ICollection<Interface> Interfaces => interfaces ?? (interfaces = GetNamespaces().SelectMany(n => InterfaceInfo.FromCodeElements(n.Members, this)).ToArray());
        
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

                    var codeClass = currentNamespace.Children.OfType<CodeClass>().FirstOrDefault();

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
            return projectItem.FileCodeModel.CodeElements.OfType<CodeNamespace>().Select(ns =>
            {
                this.currentNamespace = ns;
                return ns;
            });
        }
    }
}