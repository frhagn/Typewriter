using System;
using EnvDTE;

namespace Typewriter.Metadata.CodeDom
{
    public class LazyCodeDomTypeMetadata : CodeDomTypeMetadata
    {
        private readonly string fullName;
        private readonly CodeDomFileMetadata file;

        public LazyCodeDomTypeMetadata(string fullName, bool isNullable, CodeDomFileMetadata file) : base(null, isNullable, file)
        {
            this.fullName = fullName;
            this.file = file;
        }

        protected override CodeType CodeType => base.CodeType ?? (codeType = LoadType());

        private CodeType LoadType()
        {
            return file.GetType(fullName);
        }

        public override string Name => GetName(ExtractName(), fullName);
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
    }
}