using System;
using System.Linq;
using Typewriter.CodeModel;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.Generation.Parsing
{
    public class Extensions
    {
        [Property("(extension) string name", "The name of the $context (camelCased)")]
        public static string name(IItemInfo itemInfo)
        {
            if (itemInfo.Name.Length > 1)
                return itemInfo.Name.Substring(0, 1).ToLowerInvariant() + itemInfo.Name.Substring(1);
            return itemInfo.Name.ToLowerInvariant();
        }

        [Property("(extension) string Class", "The class name of the $context")]
        public static string Class(IFieldInfo fieldInfo)
        {
            var type = fieldInfo.Type.ToString();
            return type.EndsWith("[]") ? type.Substring(0, type.Length - 2) : type;
        }

        [Property("(extension) string Default", "The default value of the $context")]
        public static string Default(IFieldInfo fieldInfo)
        {
            var type = fieldInfo.Type.ToString();

            if (type.EndsWith("[]")) return "[]";
            if (type == "boolean") return "false";
            if (type == "number") return "0";
            if (type == "void") return "void(0)";

            return "null";
        }

        [Property("(extension) string Route", "Returns the value of the RouteAttribute of the $context and the RoutePrefixAttribute of the current class")]
        public static string Route(IMethodInfo methodInfo)
        {
            var parent = methodInfo.Parent as IItemInfo;
            var route = methodInfo.Attributes.FirstOrDefault(a => a.Name == "Route");
            var routePrefix = parent != null ? parent.Attributes.FirstOrDefault(a => a.Name == "RoutePrefix") : null;

            if (route == null && routePrefix == null) return null;
            
            return string.Concat(routePrefix != null ? routePrefix.Value + "/" : null, route != null ? route.Value : null);
        }
    }
}