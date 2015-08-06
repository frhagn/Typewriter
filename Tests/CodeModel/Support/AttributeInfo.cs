using System;

namespace Typewriter.Tests.CodeModel.Support
{
    public class AttributeInfoAttribute : Attribute
    {
        public string Parameter { get; set; }

        public AttributeInfoAttribute()
        {
        }

        public AttributeInfoAttribute(string parameter)
        {
            Parameter = parameter;
        }

        public AttributeInfoAttribute(int parameter)
        {
        }
    }

    public class AttributeTestClass
    {
        [AttributeInfo]
        public string NoParameters { get; set; }

        [AttributeInfo("parameter")]
        public string StringParameter { get; set; }

        [AttributeInfo(1)]
        public string IntParameter { get; set; }

        [AttributeInfo(2, Parameter = "parameter")]
        public string IntAndNamedParameter { get; set; }
    }
}