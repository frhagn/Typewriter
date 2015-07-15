using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a parameter.
    /// </summary>
    [Context("Parameter", "Parameters")]
    public interface Parameter : Field
    {
    }

    /// <summary>
    /// Represents a collection of parameters.
    /// </summary>
    public interface ParameterCollection : ItemCollection<Parameter>
    {
    }
}