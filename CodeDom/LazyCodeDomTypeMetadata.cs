using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;

namespace Typewriter.Metadata.CodeDom
{
    public class LazyCodeDomTypeMetadata : CodeDomTypeMetadata
    {
        private readonly string fullName;
        private readonly CodeDomFileMetadata file;

        public LazyCodeDomTypeMetadata(string fullName, bool isNullable, bool isType, CodeDomFileMetadata file) : base(null, isNullable, isType, file)
        {
            this.fullName = fullName;
            this.file = file;
        }

        protected override CodeType CodeType => base.CodeType ?? (codeType = LoadType());

        private CodeType LoadType()
        {
            return file.GetType(fullName);
        }

        public override string Name => GetName(ExtractName());
        public override string FullName => GetFullName(fullName);
        public override string Namespace => ExtractNamespace();

        private string ExtractName()
        {
            var name = fullName;

            // Remove generic arguments from containing class
            var continingIndex = name.LastIndexOf(">.", StringComparison.Ordinal);
            if (continingIndex > -1)
                name = name.Remove(0, continingIndex + 1);

            name = name.Split('<')[0];
            return name.Substring(name.LastIndexOf('.') + 1);
        }

        private string ExtractNamespace()
        {
            var name = fullName;
            var parentName = string.Empty;

            // Remove generic arguments from containing class
            var continingIndex = name.LastIndexOf(">.", StringComparison.Ordinal);
            if (continingIndex > -1)
            {
                parentName = name.Substring(0, continingIndex + 1);
                name = name.Remove(0, continingIndex + 1);
            }

            name = name.Split('<')[0];

            return parentName + name.Substring(0, name.LastIndexOf('.'));
        }

        internal static IEnumerable<string> ExtractGenericTypeNames(string name)
        {
            var list = new List<string>();

            // Remove generic arguments from containing class
            var continingIndex = name.LastIndexOf(">.", StringComparison.Ordinal);
            if (continingIndex > -1)
                name = name.Remove(0, continingIndex + 2);

            var start = name.IndexOf("<", StringComparison.Ordinal);
            var end = name.LastIndexOf(">", StringComparison.Ordinal) - (start + 1);

            if (start < 0)
            {
                return list;
            }

            var arguments = name.Substring(start + 1, end);

            var current = new StringBuilder();
            var level = 0;
            foreach (var character in arguments)
            {
                if (character == ',' && level == 0)
                {
                    list.Add(current.ToString());
                    current = new StringBuilder();
                }
                else
                {
                    if (character == '<')
                        level++;
                    else if (character == '>')
                        level--;

                    current.Append(character);
                }
            }

            if (current.Length > 0)
                list.Add(current.ToString());

            return list;
        }
    }
}