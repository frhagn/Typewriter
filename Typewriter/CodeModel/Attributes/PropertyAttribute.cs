using System;

namespace Typewriter.CodeModel.Attributes
{
    public class PropertyAttribute : System.Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public PropertyAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}