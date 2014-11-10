using System;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class EnumInfo : ItemInfo, IEnumInfo
    {
        private readonly CodeEnum codeEnum;

        public EnumInfo(CodeEnum codeEnum, FileInfo file) : base(codeEnum, file)
        {
            this.codeEnum = codeEnum;
        }

        public IEnumerable<IEnumValueInfo> Values
        {
            get { return Iterator<CodeVariable2>.Select(() => codeEnum.Members, (v, i) => new EnumValueInfo(v, file, i)); }
        }
    }
}