using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a class.
    /// </summary>
    [Context("Class", "Classes")]
    public interface Class : CodeItem
    {
        /// <summary>
        /// The namespace of the class.
        /// </summary>
        [Property("string Namespace", "The namespace of the $context")]
        string Namespace { get; }

        /// <summary>
        /// Determines if the class is generic.
        /// </summary>
        [Property("bool IsGeneric", "Determines if the $context is generic")]
        bool IsGeneric { get; }

        /// <summary>
        /// All constants defined in the class.
        /// </summary>
        [Property("collection Constants", "All constants defined in the $context")]
        ConstantCollection Constants { get; }

        /// <summary>
        /// All fields defined in the class.
        /// </summary>
        [Property("collection Fields", "All fields defined in the $context")]
        FieldCollection Fields { get; }

        /// <summary>
        /// The base class of the class.
        /// </summary>
        [Property("class BaseClass", "The base class of the $context")]
        Class BaseClass { get; }

        /// <summary>
        /// The containing class of the class if it's nested.
        /// </summary>
        [Property("class ContainingClass", "The containing class of the $context if it's nested")]
        Class ContainingClass { get; }

        /// <summary>
        /// All interfaces implemented by the class.
        /// </summary>
        [Property("collection Interfaces", "All interfaces implemented by the $context")]
        InterfaceCollection Interfaces { get; }

        /// <summary>
        /// All methods defined in the class.
        /// </summary>
        [Property("collection Methods", "All methods defined in the $context")]
        MethodCollection Methods { get; }

        /// <summary>
        /// All properties defined in the class.
        /// </summary>
        [Property("collection Properties", "All properties defined in the $context")]
        PropertyCollection Properties { get; }

        /// <summary>
        /// All generic type arguments of the class.
        /// </summary>
        [Property("collection GenericTypeArguments", "All generic type arguments of the $context")]
        TypeCollection GenericTypeArguments { get; }

        /// <summary>
        /// All classes defined in the class.
        /// </summary>
        [Property("collection NestedClasses", "All classes defined in the $context")]
        ClassCollection NestedClasses { get; }

        /// <summary>
        /// All enums defined in the class.
        /// </summary>
        [Property("collection NestedEnums", "All enums defined in the $context")]
        EnumCollection NestedEnums { get; }

        /// <summary>
        /// All interfaces defined in the class.
        /// </summary>
        [Property("collection NestedInterfaces", "All interfaces defined in the $context")]
        InterfaceCollection NestedInterfaces { get; }
    }

    /// <summary>
    /// Represents a collection of classes.
    /// </summary>
    public interface ClassCollection : ItemCollection<Class>
    {
    }
}