using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;

namespace Typewriter.CodeModel.CodeDom
{
    public class TypeInfo : ItemInfo, Type
    {
        private static readonly string[] primitiveTypes = { "string", "number", "boolean", "Date" };

        private readonly string fullName;
        private CodeType codeType;

        public TypeInfo(CodeType codeType, object parent, FileInfo file)
            : base(codeType, parent, file)
        {
            this.codeType = codeType;
            this.fullName = codeType.FullName;
        }

        public TypeInfo(string fullName, object parent, FileInfo file)
            : base(null, parent, file)
        {
            this.fullName = fullName;
        }

        protected override void Load()
        {
            if (this.codeType != null) return;

            this.codeType = file.GetType(fullName);
            this.element = this.codeType;
        }

        public override string Name
        {
            get
            {
                var name = this.fullName.Split('<')[0];
                return name.Substring(name.LastIndexOf('.') + 1);
            }
        }

        public override string Namespace
        {
            get
            {
                var name = this.fullName.Split('<')[0];
                return name.Substring(0, name.LastIndexOf('.'));
            }
        }

        public override string FullName
        {
            get { return this.fullName; }
        }

        public override bool IsGeneric
        {
            get { return FullName.IndexOf("<", StringComparison.Ordinal) > -1 || IsNullable; }
        }

        public override bool IsNullable
        {
            get { return FullName.StartsWith("System.Nullable<") || FullName.EndsWith("?"); }
        }

        public override bool IsPrimitive
        {
            get
            {
                var type = this.ToString().TrimEnd('[', ']');
                return this.IsEnum || primitiveTypes.Any(t => t == type);
            }
        }

        public override bool IsEnum
        {
            get
            {
                Load();
                return codeType.Kind == vsCMElement.vsCMElementEnum;
            }
        }

        public override bool IsEnumerable
        {
            get { return FullName != "System.String" && FullName.StartsWith("System.Collections."); }
        }

        public override string ToString()
        {
            var type = this as Type;

            if (type.IsNullable)
            {
                type = type.GenericTypeArguments.FirstOrDefault();
            }
            else if (type.IsGeneric && !type.IsEnumerable)
            {
                var types = type.GenericTypeArguments.Select(t => t.ToString());
                return string.Format("{0}<{1}>", GetTypeScriptType(type.Name), string.Join(", ", types));
            }
            else if (type.IsEnumerable)
            {
                if (type.Name.EndsWith("[]")) return GetTypeScriptType(type.Name.Substring(0, type.Name.Length - 2)) + "[]";

                type = type.GenericTypeArguments.FirstOrDefault();
                if (type != null)
                {
                    if (type.IsNullable)
                    {
                        type = type.GenericTypeArguments.FirstOrDefault();
                    }

                    return type.ToString() + "[]";
                }

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