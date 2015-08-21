using System;
using System.Collections.Generic;
using System.Linq;
using Typewriter.CodeModel.Collections;
using Typewriter.Metadata.Interfaces;
using static Typewriter.CodeModel.Helpers;

namespace Typewriter.CodeModel.Implementation
{
    public sealed class TypeImpl : Type
    {
        private readonly ITypeMetadata metadata;
        private readonly Lazy<string> lazyName;
        private readonly Lazy<string> lazyOriginalName;

        private TypeImpl(ITypeMetadata metadata, Item parent)
        {
            this.metadata = metadata;
            this.Parent = parent;
            this.lazyName = new Lazy<string>(() => GetTypeScriptName(metadata));
            this.lazyOriginalName = new Lazy<string>(() => GetOriginalName(metadata));
        }

        public override Item Parent { get; }

        public override string name => CamelCase(lazyName.Value);
        public override string Name => lazyName.Value;
        public override string OriginalName => lazyOriginalName.Value;
        public override string FullName => metadata.FullName;
        public override string Namespace => metadata.Namespace;
        public override bool IsGeneric => metadata.IsGeneric;
        public override bool IsEnum => metadata.IsEnum;
        public override bool IsEnumerable => metadata.IsEnumerable;
        public override bool IsNullable => metadata.IsNullable;
        public override bool IsTask => metadata.IsTask;
        public override bool IsPrimitive => IsPrimitive(metadata);
        public override bool IsDate => Name == "Date";
        public override bool IsGuid => FullName == "System.Guid" || FullName == "System.Guid?";
        public override bool IsTimeSpan => FullName == "System.TimeSpan" || FullName == "System.TimeSpan?";
        public override string Default => GetDefaultValue();

        private AttributeCollection attributes;
        public override AttributeCollection Attributes => attributes ?? (attributes = AttributeImpl.FromMetadata(metadata.Attributes, this));

        private TypeCollection typeArguments;
        public override TypeCollection TypeArguments => typeArguments ?? (typeArguments = TypeImpl.FromMetadata(metadata.TypeArguments, this));
        
        private string GetDefaultValue()
        {
            // Dictionary = { [key: type]: type; }
            if (Name.StartsWith("{")) return "{}";

            if (IsEnumerable) return "[]";

            if (Name == "boolean") return "false";
            if (Name == "number") return "0";
            if (Name == "void") return "void(0)";

            return "null";
        }

        public override string ToString()
        {
            return Name;
        }

        public static TypeCollection FromMetadata(IEnumerable<ITypeMetadata> metadata, Item parent)
        {
            return new TypeCollectionImpl(metadata.Select(t => new TypeImpl(t, parent)));
        }

        public static Type FromMetadata(ITypeMetadata metadata, Item parent)
        {
            return metadata == null ? null : new TypeImpl(metadata, parent);
        }
    }
}