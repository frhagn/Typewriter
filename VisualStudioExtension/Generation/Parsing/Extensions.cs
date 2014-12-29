using System;
using System.Linq;
using Typewriter.CodeModel;

namespace Typewriter.Generation.Parsing
{
    public class Extensions
    {
        public static string ClassName(IMethodInfo propertyInfo)
        {
            return ClassName(propertyInfo.Type);
        }

        public static string ClassName(IFieldInfo propertyInfo)
        {
            return ClassName(propertyInfo.Type);
        }

        public static string ClassName(IConstantInfo propertyInfo)
        {
            return ClassName(propertyInfo.Type);
        }

        public static string ClassName(IParameterInfo propertyInfo)
        {
            return ClassName(propertyInfo.Type);
        }

        public static string ClassName(IPropertyInfo propertyInfo)
        {
            return ClassName(propertyInfo.Type);
        }

        public static string ClassName(ITypeInfo propertyInfo)
        {
            var type = TypeName(propertyInfo);
            return type.EndsWith("[]") ? type.Substring(0, type.Length - 2) : type;
        }

        public static string TypeName(IMethodInfo propertyInfo)
        {
            return TypeName(propertyInfo.Type);
        }

        public static string TypeName(IFieldInfo propertyInfo)
        {
            return TypeName(propertyInfo.Type);
        }

        public static string TypeName(IConstantInfo propertyInfo)
        {
            return TypeName(propertyInfo.Type);
        }

        public static string TypeName(IParameterInfo propertyInfo)
        {
            return TypeName(propertyInfo.Type);
        }

        public static string TypeName(IPropertyInfo propertyInfo)
        {
            return TypeName(propertyInfo.Type);
        }
        
        public static string TypeName(ITypeInfo type)
        {
            if (type.IsNullable)
            {
                type = type.GenericTypeArguments.FirstOrDefault();
            }
            else if (type.IsEnumerable)
            {
                if (type.Name.EndsWith("[]")) return GetTypeScriptType(type.Name.Substring(0, type.Name.Length - 2)) + "[]";

                type = type.GenericTypeArguments.FirstOrDefault();
                if (type != null) return GetTypeScriptType(type.Name) + "[]";

                return "any[]";
            }

            return GetTypeScriptType(type.Name);
        }

        private static string GetTypeScriptType(string type)
        {
            switch (type)
            {
                case "Boolean":
                    return "boolean";
                case "String":
                case "Char":
                    return "string";
                case "Byte":
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
                    return "Date";
                case "Void":
                    return "void";
                default:
                    return type;
            }
        }
    }
}