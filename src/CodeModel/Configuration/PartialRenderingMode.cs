using System;

namespace Typewriter.Configuration
{
    /// <summary>
    /// Determines how partial classes and interfaces are rendered.
    /// </summary>
    public enum PartialRenderingMode
    {
        /// <summary>
        /// Partial types are rendered as defined in the c# source containing only the parts defined in each file.
        /// (This is the default rendering mode)
        /// </summary>
        Partial,

        /// <summary>
        /// Partial type definitions are combined to a single type acting as if the full type was 
        /// defined only once in the c# source (using the filename of the first file containing a part of the type).
        /// (Unsupported in Visual Studio 2013)
        /// </summary>
        Combined,

        /// <summary>
        /// [depricated] A combined type definition are rendered for each file containing a partial definition.
        /// (Unsupported in Visual Studio 2013)
        /// </summary>
        [Obsolete]
        Legacy
    }
}
