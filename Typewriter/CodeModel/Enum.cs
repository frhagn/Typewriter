using System;
using System.Collections.Generic;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    [Context("Enum", "Enums")]
    public interface Enum : CodeItem
    {
        [Property("string Namespace", "The namespace of the $context")]
        string Namespace { get; }

        [Property("bool IsFlags", "Determines if the $context is decorated with the Flags attribute")]
        bool IsFlags { get; }

        [Property("collection Values", "All values defined in the $context")]
        ICollection<EnumValue> Values { get; }

        [Property("class ContainingClass", "The containing class of the $context if it is nested")]
        Class ContainingClass { get; }
    }
}