using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host.Mef;

namespace Typewriter.TemplateEditor.Lexing.Roslyn
{
    internal class XmlDocumentationProvider : DocumentationProvider
    {
        private static readonly ConcurrentDictionary<string, XmlDocumentationProvider> documentationProviders = new ConcurrentDictionary<string, XmlDocumentationProvider>();

        public static XmlDocumentationProvider GetDocumentationProvider(Assembly assembly)
        {
            var path = Path.ChangeExtension(assembly.Location, "xml");
            return documentationProviders.GetOrAdd(path, p => new XmlDocumentationProvider(p));
        }

        private readonly string filePath;
        private readonly Lazy<Dictionary<string, string>> docComments;
        
        public XmlDocumentationProvider(string filePath)
        {
            this.filePath = filePath;
            this.docComments = new Lazy<Dictionary<string, string>>(CreateDocComments, true);
        }

        public override bool Equals(object obj)
        {
            var other = obj as XmlDocumentationProvider;
            return other != null && filePath == other.filePath;
        }

        public override int GetHashCode()
        {
            return filePath.GetHashCode();
        }

        protected override string GetDocumentationForSymbol(string documentationMemberId, CultureInfo preferredCulture, CancellationToken cancellationToken = default(CancellationToken))
        {
            string docComment;
            return docComments.Value.TryGetValue(documentationMemberId, out docComment) ? docComment : "";
        }

        public string GetDocumentationForSymbol(string documentationMemberId)
        {
            string docComment;
            return docComments.Value.TryGetValue(documentationMemberId, out docComment) ? docComment : "";
        }

        private Dictionary<string, string> CreateDocComments()
        {
            var commentsDictionary = new Dictionary<string, string>();
            try
            {
                var foundPath = GetDocumentationFilePath(filePath);
                if (!string.IsNullOrEmpty(foundPath))
                {
                    var document = XDocument.Load(foundPath);

                    foreach (var element in document.Descendants("member"))
                    {
                        if (element.Attribute("name") != null)
                        {
                            commentsDictionary[element.Attribute("name").Value] = string.Concat(element.Nodes());
                        }
                    }
                }
            }
            catch
            {
            }
            return commentsDictionary;
        }

        private static string GetDocumentationFilePath(string path)
        {
            if (File.Exists(path)) return path;

            var fileName = Path.GetFileName(path);
            if (fileName == null) return null;

            path = Path.Combine(Constants.ResourcesDirectory, fileName);
            if (File.Exists(path)) return path;

            path = Path.Combine(Constants.ReferenceAssembliesDirectory, @"v4.5.2", fileName);
            if (File.Exists(path)) return path;

            path = Path.Combine(Constants.ReferenceAssembliesDirectory, @"v4.5.1", fileName);
            if (File.Exists(path)) return path;

            path = Path.Combine(Constants.ReferenceAssembliesDirectory, @"v4.5", fileName);
            if (File.Exists(path)) return path;

            path = Path.Combine(Constants.ReferenceAssembliesDirectory, @"v4.6", fileName);
            if (File.Exists(path)) return path;

            return null;
        }
    }
}
