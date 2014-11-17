using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;

namespace Typewriter.CodeModel.CodeDom
{
    public class TypeInfo : ItemInfo, ITypeInfo
    {
        private static readonly Type[] primitiveTypes = { typeof(string), typeof(char), typeof(byte), typeof(sbyte), typeof(ushort), typeof(short), typeof(uint), typeof(int), typeof(ulong), typeof(long), typeof(float), typeof(double), typeof(decimal), typeof(void), typeof(bool), typeof(DateTime) };

        private readonly string fullName;
        private CodeType codeType;

        public TypeInfo(CodeType codeType, FileInfo file)
            : base(codeType, file)
        {
            this.codeType = codeType;
            this.fullName = codeType.FullName;
        }

        public TypeInfo(string fullName, FileInfo file)
            : base(null, file)
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

        public bool IsGeneric
        {
            get { return FullName.IndexOf("<", StringComparison.Ordinal) > -1 || IsNullable; }
        }

        public bool IsNullable
        {
            get { return FullName.StartsWith("System.Nullable<") || FullName.EndsWith("?"); }
        }

        public bool IsPrimitive
        {
            get
            {
                if (IsNullable)
                {
                    if (FullName.EndsWith("?")) return primitiveTypes.Any(t => t.FullName == FullName.TrimEnd('?'));
                    return primitiveTypes.Any(t => t.FullName == ExtractGenericTypeNames(FullName).First());
                }

                return primitiveTypes.Any(t => t.FullName == FullName);
            }
        }

        public bool IsEnum
        {
            get
            {
                Load();
                return codeType.Kind == vsCMElement.vsCMElementEnum;
            }
        }

        public bool IsEnumerable
        {
            //get { return FullName != "System.String" && (FullName.StartsWith("System.Collections.") || Implements(Interfaces, "System.Collections.")); }
            get { return FullName != "System.String" && FullName.StartsWith("System.Collections."); }
        }

        private static bool Implements(IEnumerable<IInterfaceInfo> interfaces, string name)
        {
            return interfaces.Any(i => i.FullName.StartsWith(name) || Implements(i.Interfaces, name));
        }

        public IEnumerable<ITypeInfo> GenericTypeArguments
        {
            get
            {
                if (IsGeneric == false) return new ITypeInfo[0];
                if (IsNullable && FullName.EndsWith("?")) return new[] { new TypeInfo(FullName.TrimEnd('?'), file) };

                return ExtractGenericTypeNames(FullName).Select(n =>
                {
                    if (n.EndsWith("[]"))
                    {
                        n = string.Format("System.Collections.Generic.ICollection<{0}>", n);
                    }
                    return new TypeInfo(n, file);
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
    }
}