using EnvDTE;

namespace Typewriter.Metadata.CodeDom
{
    public class LazyCodeDomTypeMetadata : CodeDomTypeMetadata
    {
        private readonly string fullName;
        private readonly CodeDomFileMetadata file;

        public LazyCodeDomTypeMetadata(string fullName, CodeDomFileMetadata file) : base(null, file)
        {
            this.fullName = fullName;
            this.file = file;
        }

        protected override CodeType CodeType => base.CodeType ?? (codeType = LoadType());

        private CodeType LoadType()
        {
            return file.GetType(fullName);
        }

        public override string FullName => fullName;

        public override string Name
        {
            get
            {
                var name = FullName.Split('<')[0];
                return name.Substring(name.LastIndexOf('.') + 1);
            }
        }

        public override string Namespace
        {
            get
            {
                var name = FullName.Split('<')[0];
                return name.Substring(0, name.LastIndexOf('.'));
            }
        }
    }
}