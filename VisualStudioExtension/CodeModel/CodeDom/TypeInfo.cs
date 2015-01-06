using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;

namespace Typewriter.CodeModel.CodeDom
{
    public class TypeInfo : ItemInfo, ITypeInfo
    {
        private static readonly string[] primitiveTypes = { "string", "number", "boolean", "Date" };

        private readonly string fullName;
        private CodeType codeType;

        public TypeInfo(CodeType codeType, object parent, FileInfo file) : base(codeType, parent, file)
        {
            this.codeType = codeType;
            this.fullName = codeType.FullName;
        }

        public TypeInfo(string fullName, object parent, FileInfo file) : base(null, parent, file)
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
            //get { return FullName != "System.String" && (FullName.StartsWith("System.Collections.") || Implements(Interfaces, "System.Collections.")); }
            get { return FullName != "System.String" && FullName.StartsWith("System.Collections."); }
        }

        public override string Class
        {
            get
            {
                var type = this.ToString();
                return type.EndsWith("[]") ? type.Substring(0, type.Length - 2) : type;
            }
        }

        public override string Default
        {
            get 
            {
                var type = this.ToString();
                
                if (type.EndsWith("[]")) return "[]";
                if (type == "boolean") return "false";
                if (type == "number") return "0";
                if (type == "string" || type == "Date" || this.IsEnum) return "null";
                if (type == "void") return "void(0)";

                return string.Format("new {0}()", type);
            }
        }

        //private static bool Implements(IEnumerable<IInterfaceInfo> interfaces, string name)
        //{
        //    return interfaces.Any(i => i.FullName.StartsWith(name) || Implements(i.Interfaces, name));
        //}

        public IEnumerable<ITypeInfo> GenericTypeArguments
        {
            get
            {
                if (IsGeneric == false) return new ITypeInfo[0];
                if (IsNullable && FullName.EndsWith("?")) return new[] { new TypeInfo(FullName.TrimEnd('?'), this, file) };

                return ExtractGenericTypeNames(FullName).Select(n =>
                {
                    if (n.EndsWith("[]"))
                    {
                        n = string.Format("System.Collections.Generic.ICollection<{0}>", n);
                    }
                    return new TypeInfo(n, this, file);
                });
            }
        }

        private static IEnumerable<string> ExtractGenericTypeNames(string name)
        {
            var list = new List<string>();
            var start = name.IndexOf("<", StringComparison.Ordinal);
            var end = name.LastIndexOf(">", StringComparison.Ordinal) - (start + 1);

            if (start < 0)
            {
                return list;
            }

            var arguments = name.Substring(start + 1, end);

            var current = new StringBuilder();
            var level = 0;
            foreach (var character in arguments)
            {
                if (character == ',' && level == 0)
                {
                    list.Add(current.ToString());
                    current = new StringBuilder();
                }
                else
                {
                    if (character == '<')
                        level++;
                    else if (character == '>')
                        level--;

                    current.Append(character);
                }
            }

            if (current.Length > 0)
                list.Add(current.ToString());

            return list;
        }

        public override string ToString()
        {
            var type = this as ITypeInfo;

            if (type.IsNullable)
            {
                type = type.GenericTypeArguments.FirstOrDefault();
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
                    
                    return GetTypeScriptType(type.Name) + "[]";
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