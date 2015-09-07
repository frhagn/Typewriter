//using System;

//namespace Typewriter.CodeModel.Attributes
//{
//    /// <summary>
//    /// Marks metadata properties with QuickInfo.
//    /// </summary>
//    public class PropertyAttribute : System.Attribute
//    {
//        /// <summary>
//        /// The name of the marked property.
//        /// </summary>
//        public string Name { get; private set; }

//        /// <summary>
//        /// The description of the marked property.
//        /// </summary>
//        public string Description { get; private set; }

//        /// <summary>
//        /// Determines if the property requires a templat to be recognized by the template.
//        /// </summary>
//        public bool RequireTemplate { get; private set; }

//        /// <summary>
//        /// Initializes a new instance of the PropertyAttribute.
//        /// </summary>
//        public PropertyAttribute(string name, string description, bool requireTemplate = true)
//        {
//            Name = name;
//            Description = description;
//            RequireTemplate = requireTemplate;
//        }
//    }
//}