using System;
using System.IO;
using System.Linq;
using System.Reflection;
using EnvDTE;
using Microsoft.CodeAnalysis;
using Typewriter.TemplateEditor.Lexing.Roslyn;
using Typewriter.VisualStudio;

namespace Typewriter.Generation
{
    internal static class Compiler
    {
        public static Type Compile(ProjectItem projectItem, ShadowClass shadowClass)
        {
            if (Directory.Exists(Constants.TempDirectory) == false)
            {
                Directory.CreateDirectory(Constants.TempDirectory);
            }

            var filname = Path.GetRandomFileName();
            var path = Path.Combine(Constants.TempDirectory, filname);

            var result = shadowClass.Compile(path);

            ErrorList.Clear();

            var errors = result.Diagnostics.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error || diagnostic.Severity == DiagnosticSeverity.Warning);

            var hasErrors = false;

            foreach (var error in errors)
            {
                var message = error.GetMessage();

                message = message.Replace("__Typewriter.", string.Empty);
                //message = message.Replace("__Code.", string.Empty);
                message = message.Replace("publicstatic", string.Empty);

                Log.Warn("Template error: {0} {1}", error.Id, message);

                if (error.Severity == DiagnosticSeverity.Error || error.IsWarningAsError)
                {
                    ErrorList.AddError(projectItem, message);
                    hasErrors = true;
                }
                else
                {
                    ErrorList.AddWarning(projectItem, message);
                }
            }

            if (hasErrors)
                ErrorList.Show();

            if (result.Success)
                return Assembly.LoadFrom(path).GetTypes().FirstOrDefault();

            throw new Exception("Failed to compile template.");
        }
    }
}