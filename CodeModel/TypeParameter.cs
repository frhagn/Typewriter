using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a generic type parameter.
    /// </summary>
    [Context("TypeParameter", "TypeParameters")]
    public abstract class TypeParameter : CodeItem
    {
        /// <summary>
        /// The name of the type parameter (camelCased).
        /// </summary>
        [Property("string name", "The name of the $context (camelCased).")]
        public abstract string name { get; }

        /// <summary>
        /// The name of the type parameter.
        /// </summary>
        [Property("string Name", "The name of the $context.")]
        public abstract string Name { get; }
    }

    /// <summary>
    /// Represents a collection of generic type parameters.
    /// </summary>
    public interface TypeParameterCollection : ItemCollection<TypeParameter>
    {
    }
}
