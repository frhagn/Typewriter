using System;

namespace Typewriter.CodeModel.Attributes
{
    /// <summary>
    /// Marks context metadata classes.
    /// </summary>
    public class ContextAttribute : System.Attribute
    {
        /// <summary>
        /// The name of the context.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The name of collections of the context.
        /// </summary>
        public string CollectionName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ContextAttribute.
        /// </summary>
        public ContextAttribute(string name, string collectionName)
        {
            Name = name;
            CollectionName = collectionName;
        }
    }
}