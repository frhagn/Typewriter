//using System;
//using System.CodeDom.Compiler;
//using System.Linq;
//using System.Reflection;

//namespace Typewriter.Generation.Parsing
//{
//    internal static class Compiler
//    {
//        //static Compiler()
//        //{
//        //    AppDomain.CurrentDomain.AssemblyResolve += OnCurrentDomainAssemblyResolve;
//        //}

//        internal static Type Compile(string source)
//        {
//            using (var codeProvider = CodeDomProvider.CreateProvider("CSharp"))
//            {
//                //var path = string.Format(@"{0}\{1}", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mydll.dll");
//                var parameters = new CompilerParameters { GenerateExecutable = false, GenerateInMemory = true, IncludeDebugInformation = false };
//                var referencedAssemblies = GetReferencedAssemblies();

//                parameters.ReferencedAssemblies.AddRange(referencedAssemblies);
//                var compilerResults = codeProvider.CompileAssemblyFromSource(parameters, source);

//                if (compilerResults.Errors.Count == 0)
//                {
//                    return compilerResults.CompiledAssembly.GetTypes().FirstOrDefault();
//                }

//                return null;
//            }
//        }

//        //private static Assembly OnCurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
//        //{
//        //    if (args.Name.Equals(typeof(IAttributeInfo).Assembly.FullName))
//        //        return typeof(IAttributeInfo).Assembly;

//        //    if (args.Name.Equals(typeof(AttributeInfo).Assembly.FullName))
//        //        return typeof(AttributeInfo).Assembly;

//        //    return null;
//        //    //// this is absurdly expensive...don't do this more than once, or load the assembly file in a more efficient way
//        //    //// also, if the code you're using to compile the CodeDom assembly doesn't/hasn't used the referenced assembly yet, this won't work
//        //    //// and you should use Assembly.Load(...)
//        //    //foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
//        //    //{
//        //    //    if (assembly.FullName.Equals(args.Name, StringComparison.OrdinalIgnoreCase))
//        //    //    {
//        //    //        return assembly;
//        //    //    }
//        //    //}
//        //}

//        private static string[] GetReferencedAssemblies()
//        {
//            //return AppDomain.CurrentDomain.GetAssemblies().Where(v => !v.IsDynamic).Select(a => a.Location).ToArray();

//            var assemblies = typeof(Compiler).Assembly.GetReferencedAssemblies().ToList();
//            var assemblyLocations = assemblies.Select(a => Assembly.ReflectionOnlyLoad(a.FullName).Location).ToList();

//            assemblyLocations.Add(typeof(Compiler).Assembly.Location);

//            return assemblyLocations.ToArray();
//        }
//    }
//}