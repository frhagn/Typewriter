using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a parameter.
    /// </summary>
    [Context("Parameter", "Parameters")]
    public abstract class Parameter : Field
    {
    }

    /// <summary>
    /// Represents a collection of parameters.
    /// </summary>
    public interface ParameterCollection : ItemCollection<Parameter>
    {
    }
}