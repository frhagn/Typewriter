using System.Linq;
using Type = Typewriter.CodeModel.Type;

namespace Typewriter.Extensions.Types
{
    /// <summary>
    /// Extension methods for working with types.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns the name of the type without []
        /// </summary>
        public static string ClassName(this Type type)
        {
            return type.Name.TrimEnd('[', ']');
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public static string Map(this Type type, string name)
        //{
        //    if (type.IsDate) return $"new Date({name});";
        //    if (type.IsPrimitive) return $"{name};";
        //    if (type.IsEnumerable) return $"{name} ? {name}.map(i => new {ClassName(type)}(i)) : [];";

        //    return $"{name} ? new {ClassName(type)}({name}) : null;";
        //}

        //public static string Map(this Property property, string objectName)
        //{
        //    return Map(property.Type, objectName + "." + property.name);
        //}

        /// <summary>
        /// Returns the name of the first TypeArgument of a generic type or the name of the type if it's not generic.
        /// Equivalent to $Type[$IsGeneric[$TypeArguments[$Name]][$Name]]
        /// </summary>
        public static string Unwrap(this Type type)
        {
            return type.IsGeneric ? type.TypeArguments.First().Name : type.Name;
        }
    }
}
