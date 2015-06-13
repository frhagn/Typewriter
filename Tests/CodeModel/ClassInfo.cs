using System;
using System.Collections;
using System.Collections.Generic;
using Fixie;
using Fixie.Execution;

namespace Tests.CodeModel
{
    [Test("classParameter")]
    public class Class1
    {
        public Class1()
        {
        }

        public Class1(string parameter)
        {
        }

        // Primitive types
        public bool Bool1 { get; set; }

        public char Char1 { get; set; }
        public string String1 { get; set; }

        public byte Byte1 { get; set; }
        public sbyte Sbyte1 { get; set; }

        public int Int1 { get; set; }
        public uint Uint1 { get; set; }

        public short Short1 { get; set; }
        public ushort Ushort1 { get; set; }

        public long Long1 { get; set; }
        public ulong Ulong1 { get; set; }

        public float Float1 { get; set; }
        public double Double1 { get; set; }
        public decimal Decimal1 { get; set; }

        public object Object1 { get; set; }

        public DateTime DateTime1 { get; set; }

        // Framework types
        public Guid Guid1 { get; set; } // Struct
        public Exception Exception1 { get; set; } // Class
        public IDisposable Disposable1 { get; set; } // Interface
        public ConsoleColor ConsoleColor1 { get; set; } // Enum
        public Action Action1 { get; set; } // Delegate

        // Referenced dll types
        public Case Case1 { get; set; } // Class
        public CaseBehavior CaseBehavior1 { get; set; } // Interface
        public CaseStatus CaseStatus1 { get; set; } // Enum
        public CaseBehaviorAction CaseBehaviorAction1 { get; set; } // Delegate

        // Defined types
        public Class1 Class11 { get; set; } // Class
        public Enum1 Enum11 { get; set; }

        public IEnumerable<Class1> IEnumerableClass11 { get; set; }
        public IEnumerable<IEnumerable<string>> IEnumerableIEnumerableClass11 { get; set; }
        public IEnumerable<IEnumerable<IEnumerable<string>>> IEnumerableIEnumerableIEnumerableClass11 { get; set; }
    }
}
