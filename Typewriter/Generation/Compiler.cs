using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using Typewriter.CodeModel;
using Typewriter.CodeModel.CodeDom;
using Type = System.Type;

namespace Typewriter.Generation
{
    internal static class Compiler
    {
        private static readonly string[] referencedAssemblies = GetReferencedAssemblies();

        internal static Type Compile(string source)
        {
            if (Directory.Exists(Constants.TempDirectory) == false)
            {
                Directory.CreateDirectory(Constants.TempDirectory);
            }

            using (var codeProvider = CodeDomProvider.CreateProvider("CSharp"))
            {
                var filname = string.Format("{0}.dll", Guid.NewGuid());
                var path = Path.Combine(Constants.TempDirectory, filname);

                var parameters = new CompilerParameters(referencedAssemblies, path);
                var result = codeProvider.CompileAssemblyFromSource(parameters, source);

                if (result.Errors.Count > 0) return null;
                return Assembly.LoadFrom(path).GetTypes().FirstOrDefault();
            }
        }

        private static string[] GetReferencedAssemblies()
        {
            var assemblies = typeof(Compiler).Assembly.GetReferencedAssemblies().ToList();
            var assemblyLocations = assemblies.Where(a => a.FullName.StartsWith("Typewriter") == false).Select(a => Assembly.ReflectionOnlyLoad(a.FullName).Location).ToList();

            assemblyLocations.Add(typeof(Class).Assembly.Location);
            assemblyLocations.Add(typeof(CodeDomClass).Assembly.Location);
            assemblyLocations.Add(typeof(Compiler).Assembly.Location);

            return assemblyLocations.ToArray();
        }
    }
}