using Typewriter.CodeModel.Attributes;

namespace Typewriter.CodeModel
{
    /// <summary>
    /// Represents a record.
    /// </summary>
    [Context("Record", "Records")]
    public abstract class Record : Item
    {
        /// <summary>
        /// All attributes defined on the record.
        /// </summary>
        public abstract AttributeCollection Attributes { get; }

        /// <summary>
        /// The base record of the record.
        /// </summary>
        public abstract Record BaseRecord { get; }

        /// <summary>
        /// All constants defined in the record.
        /// </summary>
        public abstract ConstantCollection Constants { get; }

        /// <summary>
        /// The containing record of the record if it's nested.
        /// </summary>
        public abstract Record ContainingRecord { get; }

        /// <summary>
        /// All delegates defined in the record.
        /// </summary>
        public abstract DelegateCollection Delegates { get; }

        /// <summary>
        /// The XML documentation comment of the record.
        /// </summary>
        public abstract DocComment DocComment { get; }

        /// <summary>
        /// All events defined in the record.
        /// </summary>
        public abstract EventCollection Events { get; }

        /// <summary>
        /// All fields defined in the record.
        /// </summary>
        public abstract FieldCollection Fields { get; }

        /// <summary>
        /// The full original name of the record including namespace and containing record names.
        /// </summary>
        public abstract string FullName { get; }

        /// <summary>
        /// All interfaces implemented by the record.
        /// </summary>
        public abstract InterfaceCollection Interfaces { get; }

        /// <summary>
        /// Determines if the record is abstract.
        /// </summary>
        public abstract bool IsAbstract { get; }

        /// <summary>
        /// Determines if the record is generic.
        /// </summary>
        public abstract bool IsGeneric { get; }

        /// <summary>
        /// All methods defined in the class.
        /// </summary>
        public abstract MethodCollection Methods { get; }

        /// <summary>
        /// The name of the record (camelCased).
        /// </summary>
        public abstract string name { get; }

        /// <summary>
        /// The name of the record.
        /// </summary>
        public abstract string Name { get; }
        
        /// <summary>
        /// The namespace of the record.
        /// </summary>
        public abstract string Namespace { get; }

        /// <summary>
        /// The parent context of the record.
        /// </summary>
        public abstract Item Parent { get; }

        /// <summary>
        /// All properties defined in the record.
        /// </summary>
        public abstract PropertyCollection Properties { get; }

        /// <summary>
        /// All generic type arguments of the record.
        /// TypeArguments are the specified arguments for the TypeParameters on a generic record e.g. &lt;string&gt;.
        /// (In Visual Studio 2013 TypeParameters and TypeArguments are the same)
        /// </summary>
        public abstract TypeCollection TypeArguments { get; }

        /// <summary>
        /// All generic type parameters of the record.
        /// TypeParameters are the type placeholders of a generic class e.g. &lt;T&gt;.
        /// (In Visual Studio 2013 TypeParameters and TypeArguments are the same)
        /// </summary>
        public abstract TypeParameterCollection TypeParameters { get; }


        /// <summary>
        /// Converts the current instance to string.
        /// </summary>
        public static implicit operator string(Record instance)
        {
            return instance.ToString();
        }

        protected abstract Type Type { get; }

        /// <summary>
        /// Converts the current instance to a Type.
        /// </summary>
        public static implicit operator Type(Record instance)
        {
            return instance?.Type;
        }
    }

    /// <summary>
    /// Represents a collection of records.
    /// </summary>
    public interface RecordCollection : ItemCollection<Record>
    {
    }
}