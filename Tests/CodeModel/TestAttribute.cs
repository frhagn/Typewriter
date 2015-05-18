using System;

namespace Tests.CodeModel
{
    public class TestAttribute : Attribute
    {
        public string Parameter { get; set; }

        public TestAttribute(string parameter)
        {
            Parameter = parameter;
        }
    }
}