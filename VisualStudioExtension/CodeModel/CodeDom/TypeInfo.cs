using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using EnvDTE;
using EnvDTE80;

namespace Typewriter.CodeModel.CodeDom
{
    public class TypeInfo : ItemInfo, ITypeInfo
    {
        private static readonly Type[] primitiveTypes =
        {
            typeof (string),
            typeof (char),
            typeof (byte),
            typeof (sbyte),
            typeof (ushort),
            typeof (short),
            typeof (uint),
            typeof (int),
            typeof (ulong),
            typeof (long),
            typeof (float),
            typeof (double),
            typeof (decimal),
            typeof (void),
            typeof (bool),
            typeof (DateTime)
        };

        private readonly CodeType codeType;

        public TypeInfo(CodeType codeType, FileInfo file) : base(codeType, file)
        {
            this.codeType = codeType;
        }

        public bool IsEnum
        {
            get { return codeType.Kind == vsCMElement.vsCMElementEnum; }
        }

        public bool IsEnumerable
        {
            get { return FullName != "System.String" && (FullName.StartsWith("System.Collections.") || FullName.EndsWith("[]") || Implements(Interfaces, "System.Collections.")); }
        }

        private bool Implements(IEnumerable<IInterfaceInfo> interfaces, string name)
        {
            return interfaces.Any(i => i.FullName.StartsWith(name) || Implements(i.Interfaces, name));
        }

        public bool IsGeneric
        {
            get
            {
                var codeClass = codeType as CodeClass2;
                if (codeClass != null)
                {
                    return codeClass.IsGeneric;
                }
                
                var codeInterface = codeType as CodeInterface2;
                if (codeInterface != null)
                {
                    return codeInterface.IsGeneric;
                }

                return IsNullable;
            }
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
                    if (FullName.EndsWith("?")) return primitiveTypes.Any(t => t.FullName == codeType.FullName.TrimEnd('?'));
                    return primitiveTypes.Any(t => t.FullName == ExtractGenericTypeNames(FullName).First());
                }

                return primitiveTypes.Any(t => t.FullName == codeType.FullName); 
            }
        }
        
        public IEnumerable<ITypeInfo> GenericTypeArguments
        {
            get
            {
                if (IsGeneric == false) return new ITypeInfo[0];
                if (IsNullable && FullName.EndsWith("?")) return new[] { file.GetType(codeType.FullName.TrimEnd('?')) };

                return ExtractGenericTypeNames(FullName).Select(file.GetType);
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