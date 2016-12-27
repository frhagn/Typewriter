using System;
using Typewriter.CodeModel;

namespace Typewriter.Configuration
{
    /// <summary>
    /// Provides settings for Typewriter Templates
    /// </summary>
    public abstract class Settings
    {
        /// <summary>
        /// Includes files in the specified project when rendering the template.
        /// </summary>
        public abstract Settings IncludeProject(string projectName);

        /// <summary>
        /// Includes files in the current project when rendering the template.
        /// </summary>
        public abstract Settings IncludeCurrentProject();

        /// <summary>
        /// Includes files in all referenced projects when rendering the template.
        /// </summary>
        public abstract Settings IncludeReferencedProjects();

        /// <summary>
        /// Includes files in all projects in the solution when rendering the template.
        /// Note: Including all projects can have a large impact on performance in large solutions.
        /// </summary>
        public abstract Settings IncludeAllProjects();

        /// <summary>
        /// Gets or sets the file extension for output files.
        /// </summary>
        public string OutputExtension { get; set; } = ".ts";

        /// <summary>
        /// Gets or sets a filename factory for the template.
        /// The factory is called for each rendered file to determine the output filename (including extension).
        /// Example: file => file.Classes.First().FullName + ".ts";
        /// </summary>
        public Func<File, string> OutputFilenameFactory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PartialRenderingMode PartialRenderingMode { get; set; } = PartialRenderingMode.Partial;
    }
}
