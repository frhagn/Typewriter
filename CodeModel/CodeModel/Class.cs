using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a class.
    /// </summary>
    [Context("Class", "Classes")]
    public abstract class Class : CodeItem
    {
        /// <summary>
        /// The name of the class (camelCased).
        /// </summary>
        //[Property("string name", "The name of the $context (camelCased).")]
        public abstract string name { get; }

        /// <summary>
        /// The name of the class.
        /// </summary>
        //[Property("string Name", "The name of the $context.")]
        public abstract string Name { get; }

        /// <summary>
        /// The full original name of the class including namespace and containing class names.
        /// </summary>
        //[Property("string FullName", "The full original name of the $context including namespace and containing class names.")]
        public abstract string FullName { get; }

        /// <summary>
        /// The namespace of the class.
        /// </summary>
        //[Property("string Namespace", "The namespace of the $context.")]
        public abstract string Namespace { get; }

        /// <summary>
        /// Determines if the class is generic.
        /// </summary>
        //[Property("bool IsGeneric", "Determines if the $context is generic.")]
        public abstract bool IsGeneric { get; }

        /// <summary>
        /// All attributes defined on the class.
        /// </summary>
        //[Property("collection Attributes", "All attributes defined on the $context.")]
        public abstract AttributeCollection Attributes { get; }

        /// <summary>
        /// All constants defined in the class.
        /// </summary>
        //[Property("collection Constants", "All constants defined in the $context.")]
        public abstract ConstantCollection Constants { get; }

        /// <summary>
        /// All fields defined in the class.
        /// </summary>
        //[Property("collection Fields", "All fields defined in the $context.")]
        public abstract FieldCollection Fields { get; }

        /// <summary>
        /// The base class of the class.
        /// </summary>
        //[Property("class BaseClass", "The base class of the $context.")]
        public abstract Class BaseClass { get; }

        /// <summary>
        /// The containing class of the class if it's nested.
        /// </summary>
        //[Property("class ContainingClass", "The containing class of the $context if it's nested.")]
        public abstract Class ContainingClass { get; }

        /// <summary>
        /// All interfaces implemented by the class.
        /// </summary>
        //[Property("collection Interfaces", "All interfaces implemented by the $context.")]
        public abstract InterfaceCollection Interfaces { get; }

        /// <summary>
        /// All methods defined in the class.
        /// </summary>
        //[Property("collection Methods", "All methods defined in the $context.")]
        public abstract MethodCollection Methods { get; }

        /// <summary>
        /// All properties defined in the class.
        /// </summary>
        //[Property("collection Properties", "All properties defined in the $context.")]
        public abstract PropertyCollection Properties { get; }

        /// <summary>
        /// All generic type parameters of the class.
        /// </summary>
        //[Property("collection TypeParameters", "All generic type parameters of the $context.", requireTemplate: false)]
        public abstract TypeParameterCollection TypeParameters { get; }


        /// <summary>
        /// All generic type parameters of the class.
        /// </summary>
        //[Property("collection TypeArguments", "All generic type arguments of the $context.")]
        public abstract TypeCollection TypeArguments { get; }

        /// <summary>
        /// All classes defined in the class.
        /// </summary>
        //[Property("collection NestedClasses", "All classes defined in the $context.")]
        public abstract ClassCollection NestedClasses { get; }

        /// <summary>
        /// All enums defined in the class.
        /// </summary>
        //[Property("collection NestedEnums", "All enums defined in the $context.")]
        public abstract EnumCollection NestedEnums { get; }

        /// <summary>
        /// All interfaces defined in the class.
        /// </summary>
        //[Property("collection NestedInterfaces", "All interfaces defined in the $context.")]
        public abstract InterfaceCollection NestedInterfaces { get; }

        /// <summary>
        /// Converts the current instance to string.
        /// </summary>
        public static implicit operator string (Class instance)
        {
            return instance.ToString();
        }
    }

    /// <summary>
    /// Represents a collection of classes.
    /// </summary>
    public interface ClassCollection : ItemCollection<Class>
    {
    }
}