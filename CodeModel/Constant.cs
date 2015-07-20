using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a constant.
    /// </summary>
    [Context("Constant", "Constants")]
    public abstract class Constant : Field
    {
        /// <summary>
        /// Converts the current instance to string.
        /// </summary>
        public static implicit operator string (Constant instance)
        {
            return instance.ToString();
        }
    }

    /// <summary>
    /// Represents a collection of constants.
    /// </summary>
    public interface ConstantCollection : ItemCollection<Constant>
    {
    }
}