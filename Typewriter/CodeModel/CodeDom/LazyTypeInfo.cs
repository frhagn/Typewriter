using EnvDTE;

namespace Typewriter.CodeModel.CodeDom
{
    public class LazyTypeInfo : TypeInfo
    {
        private readonly string fullName;

        public LazyTypeInfo(string fullName, Item parent) : base(null, parent)
        {
            this.fullName = fullName;
        }

        protected override CodeType CodeType => base.CodeType ?? (codeType = LoadType());

        private CodeType LoadType()
        {
            var parent = Parent;
            while (parent is CodeItem)
            {
                parent = ((CodeItem)parent).Parent;
            }

            return ((FileInfo)parent).GetType(fullName);
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