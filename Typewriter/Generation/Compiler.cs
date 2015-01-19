using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Typewriter.Generation
{
    internal static class Compiler
    {
        private static readonly string[] referencedAssemblies = GetReferencedAssemblies();

        internal static Type Compile(string source)
        {
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
            var assemblyLocations = assemblies.Select(a => Assembly.ReflectionOnlyLoad(a.FullName).Location).ToList();

            assemblyLocations.Add(typeof(Compiler).Assembly.Location);

            return assemblyLocations.ToArray();
        }
    }
}