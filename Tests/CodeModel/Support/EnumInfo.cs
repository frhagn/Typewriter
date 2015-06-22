using System;

namespace Typewriter.Tests.CodeModel.Support
{
    [AttributeInfo]
    public enum EnumInfo
    {
        [AttributeInfo]
        ValueA,
        ValueB,
        ValueC = 5,
        ValueD
    }

    [Flags]
    public enum FlagsEnumInfo
    {
        ValueA = 1,
        ValueB = 2,
        ValueC = 4
    }

    public class EnumContiningClassInfo
    {
        public enum NestedEnumInfo
        {
        }
    }
}
