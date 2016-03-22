using System;

namespace Typewriter.Tests.CodeModel.Support
{
    /// <summary>
    /// summary
    /// </summary>
    [AttributeInfo]
    public enum EnumInfo
    {
        [AttributeInfo]
        ValueA,
        ValueB,
        ValueC = 5,
        ValueD,
        ValueE = 'A',
        ValueF
    }

    [Flags]
    public enum FlagsEnumInfo
    {
        ValueA = 1,
        ValueB = 2,
        ValueC = 4
    }

    public enum HexEnumInfo : byte
    {
        ValueA = 0x01,
        ValueB = 0x02,
        ValueC = 0x03
    }

    public class EnumContiningClassInfo
    {
        public enum NestedEnumInfo
        {
        }
    }
}
