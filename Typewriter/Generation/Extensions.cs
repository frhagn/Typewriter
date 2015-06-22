using System.Linq;
using Typewriter.CodeModel;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.Generation
{
    public static class Extensions
    {
        [Property("(extension) string name", "The name of the $context (camelCased)")]
        public static string name(CodeItem itemInfo)
        {
            if (itemInfo.Name.Length > 1)
                return itemInfo.Name.Substring(0, 1).ToLowerInvariant() + itemInfo.Name.Substring(1);
            return itemInfo.Name.ToLowerInvariant();
        }

        [Property("(extension) string Class", "The class name of the $context")]
        public static string Class(Field fieldInfo)
        {
            return Class(fieldInfo.Type);
        }

        [Property("(extension) string Default", "The default value of the $context")]
        public static string Default(Field fieldInfo)
        {
            return Default(fieldInfo.Type);
        }

        [Property("(extension) string Type", "The type name")]
        public static string Type(this Type typeInfo)
        {
            if (typeInfo.IsNullable)
                typeInfo = typeInfo.GenericTypeArguments.First();

            if (typeInfo.IsEnumerable == false)
                return GetTypeScriptType(typeInfo.Name);

            typeInfo = typeInfo.GenericTypeArguments.FirstOrDefault();

            if (typeInfo != null)
            {
                if (typeInfo.IsNullable || typeInfo.IsEnumerable)
                    return Type(typeInfo) + "[]";

                return GetTypeScriptType(typeInfo.Name) + "[]";
            }

            return "any[]";
        }

        private static string GetTypeScriptType(string type)
        {
            switch (type)
            {
                case "Boolean":
                    return "boolean";
                case "String":
                case "Char":
                case "Guid":
                case "TimeSpan":
                    return "string";
                case "Byte":
                case "SByte":
                case "Int16":
                case "Int32":
                case "Int64":
                case "UInt16":
                case "UInt32":
                case "UInt64":
                case "Single":
                case "Double":
                case "Decimal":
                    return "number";
                case "DateTime":
                case "DateTimeOffset":
                    return "Date";
                case "Void":
                    return "void";
                default:
                    return type;
            }
        }

        [Property("(extension) string Class", "The class name of the $context")]
        public static string Class(Type typeInfo)
        {
            var type = Type(typeInfo);
            return type.EndsWith("[]") ? type.Substring(0, type.Length - 2) : type;
        }

        [Property("(extension) bool IsDate", "Determines if the $context is a DateTime")]
        public static bool IsDate(Field fieldInfo)
        {
            var type = Type(fieldInfo.Type);
            return type == "Date";
        }

        [Property("(extension) string Default", "The default value of the $context")]
        public static string Default(Type typeInfo)
        {
            var type = Type(typeInfo);

            if (type.EndsWith("[]")) return "[]";
            if (type == "boolean") return "false";
            if (type == "number") return "0";
            if (type == "void") return "void(0)";

            return "null";
        }

        [Property("(extension) string Route", "Returns the value of the RouteAttribute of the $context and the RoutePrefixAttribute of the current class")]
        public static string Route(Method methodInfo)
        {
            var parent = methodInfo.Parent as CodeItem;
            var route = methodInfo.Attributes.FirstOrDefault(a => a.Name == "Route");
            var routePrefix = parent?.Attributes.FirstOrDefault(a => a.Name == "RoutePrefix");

            if (route == null && routePrefix == null) return null;
            
            return string.Concat(routePrefix != null ? routePrefix.Value + "/" : null, route?.Value);
        }
   
    }
}