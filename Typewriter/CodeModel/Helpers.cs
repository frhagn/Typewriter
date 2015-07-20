using System.Linq;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.CodeModel
{
    public static class Helpers
    {
        public static string CamelCase(string name)
        {
            if (name.Length > 1)
                return name.Substring(0, 1).ToLowerInvariant() + name.Substring(1);
            return name.ToLowerInvariant();
        }

        public static string GetTypeScriptName(ITypeMetadata metadata)
        {
            if (metadata == null)
                return "any";

            if (metadata.IsNullable)
                return GetTypeScriptName(metadata.GenericTypeArguments.First());

            if (metadata.IsEnumerable)
            {
                var genericTypeArguments = metadata.GenericTypeArguments.ToList();

                if (genericTypeArguments.Count == 1)
                    return GetTypeScriptName(genericTypeArguments.FirstOrDefault()) + "[]";

                if (genericTypeArguments.Count == 2)
                {
                    var key = GetTypeScriptName(genericTypeArguments[0]);
                    var value = GetTypeScriptName(genericTypeArguments[1]);

                    return string.Concat("{ [key: ", key, "]: ", value, "; }");
                }

                return "any";
            }

            if (metadata.IsGeneric)
                return metadata.Name + string.Concat("<", string.Join(", ", metadata.GenericTypeArguments.Select(GetTypeScriptName)), ">");

            return GetTypeScriptTypeName(metadata.Name);
        }

        private static string GetTypeScriptTypeName(string type)
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
    }
}
