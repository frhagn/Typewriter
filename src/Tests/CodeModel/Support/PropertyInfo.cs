using System;
using System.Collections;
using System.Collections.Generic;

namespace Typewriter.Tests.CodeModel.Support
{
    public class PropertyInfo
    {
        /// <summary>
        /// summary
        /// </summary>
        public string GetterOnly { get; }
        public string SetterOnly { set { } }
        public string PrivateGetter { private get; set; }
        public string PrivateSetter { get; private set; }

        // Primitive types
        [AttributeInfo]
        public bool Bool { get; set; }
        public char Char { get; set; }
        public string String { get; set; }
        public byte Byte { get; set; }
        public sbyte Sbyte { get; set; }
        public int Int { get; set; }
        public uint Uint { get; set; }
        public short Short { get; set; }
        public ushort Ushort { get; set; }
        public long Long { get; set; }
        public ulong Ulong { get; set; }
        public float Float { get; set; }
        public double Double { get; set; }
        public decimal Decimal { get; set; }

        // Special types
        public DateTime DateTime { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public Guid Guid { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public object Object { get; set; }
        public dynamic Dynamic { get; set; }

        // Enums
        public ConsoleColor Enum { get; set; }
        public ConsoleColor? NullableEnum1 { get; set; }
        public Nullable<ConsoleColor> NullableEnum2 { get; set; }

        public Exception Exception { get; set; } // Class

        // Untyped collections
        public Array Array { get; set; }
        public IEnumerable Enumerable { get; set; }

        // Typed collections
        public string[] StringArray { get; set; }
        public IEnumerable<string> EnumerableString { get; set; }
        public List<string> ListString { get; set; }

        // Nullable
        public int? NullableInt1 { get; set; }
        public Nullable<int> NullableInt2 { get; set; }
        public IEnumerable<int> EnumerableInt { get; set; }
        public IEnumerable<int?> EnumerableNullableInt { get; set; }

        // Defined types
        public ClassInfo Class { get; set; }
        public BaseClassInfo BaseClass { get; set; }
        public GenericClassInfo<string> GenericClass { get; set; }
        public IInterfaceInfo Interface { get; set; }
    }

    public class GenericPropertyInfo<T>
    {
        public T Generic { get; set; }
        public IEnumerable<T> EnumerableGeneric { get; set; }
    }
}
