using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class EnumInfo : ItemInfo, IEnumInfo
    {
        private readonly CodeEnum codeEnum;

        public EnumInfo(CodeEnum codeEnum, object parent, FileInfo file) : base(codeEnum, parent, file)
        {
            this.codeEnum = codeEnum;
        }

        public ICollection<IEnumValueInfo> Values
        {
            get { return Iterator<CodeVariable2>.Select(() => codeEnum.Members, (v, i) => new EnumValueInfo(v, this, file, i)).ToArray(); }
        }
    }
}