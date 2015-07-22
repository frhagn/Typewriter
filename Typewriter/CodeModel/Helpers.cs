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

            return ExtractTypeScriptName(metadata);
        }

        private static string ExtractTypeScriptName(ITypeMetadata metadata)
        {
            var fullName = metadata.IsNullable ? metadata.FullName.TrimEnd('?') : metadata.FullName;

            switch (fullName)
            {
                case "System.Boolean":
                    return "boolean";
                case "System.String":
                case "System.Char":
                case "System.Guid":
                case "System.TimeSpan":
                    return "string";
                case "System.Byte":
                case "System.SByte":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.Single":
                case "System.Double":
                case "System.Decimal":
                    return "number";
                case "System.DateTime":
                case "System.DateTimeOffset":
                    return "Date";
                case "System.Void":
                    return "void";
            }

            return metadata.IsNullable ? metadata.Name.TrimEnd('?') : metadata.Name;
        }
    }
}
