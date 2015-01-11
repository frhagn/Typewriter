using System;

namespace Typewriter.CodeModel.Attributes
{
    public class ContextAttribute : Attribute
    {
        public string Name { get; set; }

        public ContextAttribute(string name)
        {
            Name = name;
        }
    }
}