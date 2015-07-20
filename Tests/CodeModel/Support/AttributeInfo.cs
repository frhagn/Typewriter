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
    }
}