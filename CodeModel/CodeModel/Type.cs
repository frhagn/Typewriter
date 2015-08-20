using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a type.
    /// </summary>
    [Context("Type", "Types")]
    public abstract class Type : CodeItem
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
        /// The original C# name of the type.
        /// </summary>
        //[Property("string Name", "The name of the $context.")]
        public abstract string OriginalName { get; }

        /// <summary>
        /// The namespace of the class.
        /// </summary>
        //[Property("string Namespace", "The namespace of the $context.")]
        public abstract string Namespace { get; }

        /// <summary>
        /// All attributes defined on the class.
        /// </summary>
        //[Property("collection Attributes", "All attributes defined on the $context.")]
        public abstract AttributeCollection Attributes { get; }

        /// <summary>
        /// All generic type parameters of the class.
        /// </summary>
        //[Property("collection TypeArguments", "All generic type arguments of the $context.")]
        public abstract TypeCollection TypeArguments { get; }

        /// <summary>
        /// Determines if the class is generic.
        /// </summary>
        //[Property("bool IsGeneric", "Determines if the $context is generic.")]
        public abstract bool IsGeneric { get; }

        /// <summary>
        /// Determines if the type is an enum.
        /// </summary>
        //[Property("bool IsEnum", "Determines if the $context is an enum.")]
        public abstract bool IsEnum { get; }

        /// <summary>
        /// Determines if the type is enumerable.
        /// </summary>
        //[Property("bool IsEnumerable", "Determines if the $context is enumerable.")]
        public abstract bool IsEnumerable { get; }

        /// <summary>
        /// Determines if the type is nullable.
        /// </summary>
        //[Property("bool IsNullable", "Determines if the $context is nullable.")]
        public abstract bool IsNullable { get; }

        /// <summary>
        /// Determines if the type is primitive.
        /// </summary>
        //[Property("bool IsPrimitive", "Determines if the $context is primitive.")]
        public abstract bool IsPrimitive { get; }

        /// <summary>
        /// Determines if the type is a DateTime.
        /// </summary>
        //[Property("bool IsDate", "Determines if the $context is a DateTime.")]
        public abstract bool IsDate { get; }

        /// <summary>
        /// Determines if the type is a Guid.
        /// </summary>
        //[Property("bool IsGuid", "Determines if the $context is a Guid.")]
        public abstract bool IsGuid { get; }

        /// <summary>
        /// Determines if the type is a TimeSpan.
        /// </summary>
        //[Property("bool IsTimeSpan", "Determines if the $context is a TimeSpan.")]
        public abstract bool IsTimeSpan { get; }

        /// <summary>
        /// The default value of the type.
        /// </summary>
        //[Property("string Default", "The default value of the $context.")]
        public abstract string Default { get; }

        /// <summary>
        /// Converts the current instance to string.
        /// </summary>
        public static implicit operator string (Type instance)
        {
            return instance.ToString();
        }
    }

    /// <summary>
    /// Represents a collection of types.
    /// </summary>
    public interface TypeCollection : ItemCollection<Type>
    {
    }
}