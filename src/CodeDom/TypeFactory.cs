using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EnvDTE;

namespace Typewriter.Metadata.CodeDom
{
    public class TypeFactory
    {
        private static readonly object locker = new object();
        private readonly IDictionary<string, CodeType> typeCache = new Dictionary<string, CodeType>();

        private readonly ProjectItem projectItem;

        public TypeFactory(ProjectItem projectItem)
        {
            this.projectItem = projectItem;
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

                    var currentNamespace = projectItem.FileCodeModel.CodeElements.OfType<CodeNamespace>().FirstOrDefault();
                    var codeClass = currentNamespace != null ? currentNamespace.Children.OfType<CodeClass>().FirstOrDefault() :
                        projectItem.FileCodeModel.CodeElements.OfType<CodeClass>().FirstOrDefault();

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
                //Log.Debug("GetType({0}) completed in {1} ms", fullName, stopwatch.ElapsedMilliseconds);

                return typeInfo;
            }
        }

    }
}