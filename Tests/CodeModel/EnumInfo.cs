using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.CodeModel
{
    [Test("enumParameter"), Flags]
    public enum Enum1
    {
        ValueA,
        [Test("valueParameter")]
        ValueB = 2,
        ValueC = 1
    }
}
