using System;
using System.Collections;
using System.Collections.Generic;

namespace Tests.CodeModel.TestData
{
    public class PropertyInfo
    {
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

        public object Object { get; set; }
        public DateTime DateTime { get; set; }

        // Framework types
        public Guid Guid { get; set; } // Struct
        public Exception Exception { get; set; } // Class
        public IDisposable Disposable { get; set; } // Interface
        public ConsoleColor ConsoleColor { get; set; } // Enum
        public Action Action { get; set; } // Delegate

        // Defined types
        public ClassInfo Class { get; set; }
        public IInterfaceInfo Interface { get; set; }
        public EnumInfo Enum { get; set; }

        // Untyped collections
        public Array Array { get; set; }
        public IEnumerable Enumerable { get; set; }

        // Typed collections
        public string[] StringArray { get; set; }
        public IEnumerable<string> EnumerableString { get; set; }
        public IEnumerable<IEnumerable<string>> EnumerableEnumerableString { get; set; }
    }

    public class GenericPropertyInfo<T>
    {
        public T Generic { get; set; }
        public IEnumerable<T> EnumerableGeneric { get; set; }
        public IEnumerable<IEnumerable<T>> EnumerableEnumerableGeneric { get; set; }
    }
}
