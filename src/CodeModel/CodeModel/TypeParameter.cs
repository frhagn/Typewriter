using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a generic type parameter.
    /// </summary>
    [Context("TypeParameter", "TypeParameters")]
    public abstract class TypeParameter : Item
    {
        /// <summary>
        /// The name of the type parameter (camelCased).
        /// </summary>
        public abstract string name { get; }

        /// <summary>
        /// The name of the type parameter.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The parent context of the type parameter.
        /// </summary>
        public abstract Item Parent { get; }
    }

    /// <summary>
    /// Represents a collection of generic type parameters.
    /// </summary>
    public interface TypeParameterCollection : ItemCollection<TypeParameter>, IStringConvertable
    {
    }
}
