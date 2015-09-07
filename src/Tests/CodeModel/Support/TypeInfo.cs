using System;
using System.Collections;
using System.Collections.Generic;

namespace Typewriter.Tests.CodeModel.Support
{
    public class TypeInfo
    {
        public ClassInfo Class { get; set; }
        public BaseClassInfo BaseClass { get; set; }
        public GenericClassInfo<string> GenericClass { get; set; }
        public InheritGenericClassInfo InheritGenericClass { get; set; }
    }
}
