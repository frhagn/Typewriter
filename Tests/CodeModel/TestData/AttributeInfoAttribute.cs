using System;

namespace Tests.CodeModel.TestData
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
    }
}