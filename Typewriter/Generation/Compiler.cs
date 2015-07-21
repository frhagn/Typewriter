using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Typewriter.TemplateEditor.Lexing.Roslyn;
using Typewriter.VisualStudio;

namespace Typewriter.Generation
{
    internal static class Compiler
    {
        public static Type Compile(ShadowClass shadowClass)
        {
            if (Directory.Exists(Constants.TempDirectory) == false)
            {
                Directory.CreateDirectory(Constants.TempDirectory);
            }

            var filname = Path.GetRandomFileName();
            var path = Path.Combine(Constants.TempDirectory, filname);

            var result = shadowClass.Compile(path);

            if (result.Success)
                return Assembly.LoadFrom(path).GetTypes().FirstOrDefault();

            var errors = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

            foreach (var error in errors)
            {
                Log.Error("Template error: {0} {1}", error.Id, error.GetMessage());
            }

            throw new Exception("Failed to compile template.");
        }
    }
}