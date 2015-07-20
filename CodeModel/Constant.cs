using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a constant.
    /// </summary>
    [Context("Constant", "Constants")]
    public abstract class Constant : Field
    {
    }

    /// <summary>
    /// Represents a collection of constants.
    /// </summary>
    public interface ConstantCollection : ItemCollection<Constant>
    {
    }
}