using System;

namespace Typewriter.CodeModel.Attributes
{
    public class ContextAttribute : Attribute
    {
        public string Name { get; set; }
        public string CollectionName { get; set; }

        public ContextAttribute(string name, string collectionName)
        {
            Name = name;
            CollectionName = collectionName;
        }
    }
}