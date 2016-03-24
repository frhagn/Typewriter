using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents an XML documentation comment.
    /// </summary>
    [Context("DocComment", "DocComments")]
    public abstract class DocComment : Item
    {
        /// <summary>
        /// The contents of the summary tag.
        /// </summary>
        public abstract string Summary { get; }

        /// <summary>
        /// The contents of the returns tag.
        /// </summary>
        public abstract string Returns { get; }

        /// <summary>
        /// All parameter tags of the documentation comment.
        /// </summary>
        public abstract ParameterCommentCollection Parameters { get; }

        /// <summary>
        /// The parent context of the documentation comment.
        /// </summary>
        public abstract Item Parent { get; }
        
        /// <summary>
        /// Converts the current instance to string.
        /// </summary>
        public static implicit operator string(DocComment instance)
        {
            return instance.ToString();
        }
    }

    /// <summary>
    /// Represents an XML documentation comment parameter tag.
    /// </summary>
    [Context("ParameterComment", "ParameterComments")]
    public abstract class ParameterComment : Item
    {
        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The parameter description.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// The parent context of the documentation comment parameter.
        /// </summary>
        public abstract Item Parent { get; }

        /// <summary>
        /// Converts the current instance to string.
        /// </summary>
        public static implicit operator string(ParameterComment instance)
        {
            return instance.ToString();
        }
    }

    /// <summary>
    /// Represents a collection of parameter comments.
    /// </summary>
    public interface ParameterCommentCollection : ItemCollection<ParameterComment>
    {
    }
}