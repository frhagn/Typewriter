using System;

namespace Tests.CodeModel.TestData
{
    [AttributeInfo("enumParameter"), Flags]
    public enum EnumInfo
    {
        ValueA,
        [AttributeInfo("valueParameter")]
        ValueB = 2,
        ValueC = 1
    }
}
