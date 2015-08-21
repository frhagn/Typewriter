using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a type.
    /// </summary>
    [Context("Type", "Types")]
    public abstract class Type : Item
    {
        /// <summary>
        /// All attributes defined on the type.
        /// </summary>
        public abstract AttributeCollection Attributes { get; }

        /// <summary>
        /// The default value of the type.
        /// </summary>
        public abstract string Default { get; }

        /// <summary>
        /// The full original name of the type including namespace and containing class names.
        /// </summary>
        public abstract string FullName { get; }

        /// <summary>
        /// Determines if the type is a DateTime.
        /// </summary>
        public abstract bool IsDate { get; }

        /// <summary>
        /// Determines if the type is an enum.
        /// </summary>
        public abstract bool IsEnum { get; }

        /// <summary>
        /// Determines if the type is enumerable.
        /// </summary>
        public abstract bool IsEnumerable { get; }

        /// <summary>
        /// Determines if the class is generic.
        /// </summary>
        public abstract bool IsGeneric { get; }

        /// <summary>
        /// Determines if the type is a Guid.
        /// </summary>
        public abstract bool IsGuid { get; }

        /// <summary>
        /// Determines if the type is nullable.
        /// </summary>
        public abstract bool IsNullable { get; }

        /// <summary>
        /// Determines if the type is primitive.
        /// </summary>
        public abstract bool IsPrimitive { get; }

        /// <summary>
        /// Determines if the type is a Task.
        /// </summary>
        public abstract bool IsTask { get; }

        /// <summary>
        /// Determines if the type is a TimeSpan.
        /// </summary>
        public abstract bool IsTimeSpan { get; }

        /// <summary>
        /// The name of the class (camelCased).
        /// </summary>
        public abstract string name { get; }

        /// <summary>
        /// The name of the class.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The namespace of the class.
        /// </summary>
        public abstract string Namespace { get; }
        
        /// <summary>
        /// The original C# name of the type.
        /// </summary>
        public abstract string OriginalName { get; }

        /// <summary>
        /// The parent context of the type.
        /// </summary>
        public abstract Item Parent { get; }

        /// <summary>
        /// All generic type parameters of the class.
        /// </summary>
        public abstract TypeCollection TypeArguments { get; }

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
    public interface TypeCollection : ItemCollection<Type>, IStringConvertable
    {
    }
}