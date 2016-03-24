using System;
using System.Threading.Tasks;

namespace Typewriter.Tests.CodeModel.Support
{
    public delegate void Delegate();
    public delegate void GenericDelegate<T>();

    public class DelegateInfo
    {
        /// <summary>
        /// summary
        /// </summary>
        /// <param name="parameter">param</param>
        [AttributeInfo]
        public delegate void Delegate([AttributeInfo] string parameter);
        public delegate T Generic<T>(T parameter);
        public delegate System.Threading.Tasks.Task Task();
        public delegate Task<string> TaskString();
        public delegate Task<Nullable<int>> TaskNullableInt();
    }

    public class GenericDelegateInfo<T>
    {
        public delegate T Delegate(T parameter);
        public delegate T1 Generic<T1>(T1 parameter1, T parameter2);
    }
}
