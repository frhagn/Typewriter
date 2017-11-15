using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.CodeModel
{
    public static class Helpers
    {
        public static string CamelCase(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            if (char.IsUpper(s[0]) == false) return s;

            var chars = s.ToCharArray();

            for (var i = 0; i < chars.Length; i++)
            {
                if (i == 1 && char.IsUpper(chars[i]) == false)
                {
                    break;
                }

                var hasNext = (i + 1 < chars.Length);
                if (i > 0 && hasNext && char.IsUpper(chars[i + 1]) == false)
                {
                    break;
                }

                chars[i] = char.ToLowerInvariant(chars[i]);
            }

            return new string(chars);
        }

        public static string GetTypeScriptName(ITypeMetadata metadata)
        {
            if (metadata == null)
                return "any";

            if (metadata.IsEnumerable)
            {
                var typeArguments = metadata.TypeArguments.ToList();

                if (typeArguments.Count == 0)
                {
                    if (metadata.BaseClass != null && metadata.BaseClass.IsGeneric)
                    {
                        typeArguments = metadata.BaseClass.TypeArguments.ToList();
                    }
                    else
                    {
                        var genericInterface = metadata.Interfaces.FirstOrDefault(i => i.IsGeneric);
                        if (genericInterface != null)
                            typeArguments = genericInterface.TypeArguments.ToList();
                    }

                    if (typeArguments.Any(t => t.FullName == metadata.FullName))
                    {
                        return "any[]";
                    }
                }

                if (typeArguments.Count == 1)
                    return GetTypeScriptName(typeArguments.FirstOrDefault()) + "[]";

                if (typeArguments.Count == 2)
                {
                    var key = GetTypeScriptName(typeArguments[0]);
                    var value = GetTypeScriptName(typeArguments[1]);

                    return string.Concat("{ [key: ", key, "]: ", value, "; }");
                }

                return "any[]";
            }

            if (metadata.IsValueTuple)
            {
                var types = string.Join(", ", metadata.TupleElements.Select(p => $"{p.Name}: {GetTypeScriptName(p.Type)}"));
                return $"{{ {types} }}";
            }

            if (metadata.IsGeneric)
                return metadata.Name + string.Concat("<", string.Join(", ", metadata.TypeArguments.Select(GetTypeScriptName)), ">");

            return ExtractTypeScriptName(metadata);
        }

        public static string GetOriginalName(ITypeMetadata metadata)
        {
            var name = metadata.Name;
            var fullName = metadata.IsNullable ? metadata.FullName.TrimEnd('?') : metadata.FullName;

            if (primitiveTypes.ContainsKey(fullName))
                name = primitiveTypes[fullName] + (metadata.IsNullable ? "?" : string.Empty);

            return name;
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
                case "System.Object":
                case "dynamic":
                    return "any";
            }

            return metadata.IsNullable ? metadata.Name.TrimEnd('?') : metadata.Name;
        }

        public static bool IsPrimitive(ITypeMetadata metadata)
        {
            var fullName = metadata.FullName;

            if (metadata.IsNullable)
            {
                fullName = fullName.TrimEnd('?');
            }
            else if (metadata.IsEnumerable)
            {
                var innerType = metadata.TypeArguments.FirstOrDefault();
                if (innerType != null)
                {
                    fullName = innerType.IsNullable ? innerType.FullName.TrimEnd('?') : innerType.FullName;
                }
                else
                {
                    return false;
                }
            }

            return metadata.IsEnum || primitiveTypes.ContainsKey(fullName);
        }

        private static readonly Dictionary<string, string> primitiveTypes = new Dictionary<string, string>
        {
            { "System.Boolean", "bool" },
            { "System.Byte", "byte" },
            { "System.Char", "char" },
            { "System.Decimal", "decimal" },
            { "System.Double", "double" },
            { "System.Int16", "short" },
            { "System.Int32", "int" },
            { "System.Int64", "long" },
            { "System.SByte", "sbyte" },
            { "System.Single", "float" },
            { "System.String", "string" },
            { "System.UInt32", "uint" },
            { "System.UInt16", "ushort" },
            { "System.UInt64", "ulong" },

            { "System.DateTime", "DateTime" },
            { "System.DateTimeOffset", "DateTimeOffset" },
            { "System.Guid", "Guid" },
            { "System.TimeSpan", "TimeSpan" },
        };
    }
}
