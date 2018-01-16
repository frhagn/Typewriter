using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Typewriter.Metadata.Interfaces;

namespace Typewriter.Metadata.CodeDom
{
    public class CodeDomEnumValueMetadata : IEnumValueMetadata
    {
        private static readonly Int64Converter _converter = new Int64Converter();

        private readonly CodeVariable2 codeVariable;
        private readonly CodeDomFileMetadata file;
        private readonly long value;

        private CodeDomEnumValueMetadata(CodeVariable2 codeVariable, CodeDomFileMetadata file, long value)
        {
            this.codeVariable = codeVariable;
            this.file = file;
            this.value = value;
        }

        public string DocComment => codeVariable.DocComment;
        public string Name => codeVariable.Name;
        public string FullName => codeVariable.FullName;
        public long Value => value;
        public IEnumerable<IAttributeMetadata> Attributes => CodeDomAttributeMetadata.FromCodeElements(codeVariable.Attributes);
        
        internal static IEnumerable<IEnumValueMetadata> FromCodeElements(CodeElements codeElements, CodeDomFileMetadata file)
        {
            long value = -1;

            foreach (var codeVariable in codeElements.OfType<CodeVariable2>())
            {
                if (codeVariable.InitExpression == null)
                    value++;
                else
                {
                    string initExpression = codeVariable.InitExpression.ToString();

                    if (long.TryParse(initExpression, out value) == false)
                    {
                        // Handle init expressions from char constants e.g. 'A' = 65
                        if (initExpression.Length == 3 && initExpression.StartsWith("'") && initExpression.EndsWith("'"))
                        {
                            value = initExpression[1];
                        }
                        else if (initExpression.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                        {
                            var convertedValue = _converter.ConvertFromString(initExpression);
                            value = (long?)convertedValue ?? -1;
                        }
                        else
                        {
                            value = -1;
                        }
                    }
                }

                yield return new CodeDomEnumValueMetadata(codeVariable, file, value);
            }
        }
    }
}