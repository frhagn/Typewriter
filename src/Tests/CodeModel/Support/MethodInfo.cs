using System;
using System.Threading.Tasks;

namespace Typewriter.Tests.CodeModel.Support
{
    public class MethodInfo
    {
        /// <summary>
        /// summary
        /// </summary>
        /// <returns>returns</returns>
        /// <param name="parameter">param</param>
        [AttributeInfo]
        public void Method([AttributeInfo]string parameter)
        {
        }

        public T Generic<T>(T parameter)
        {
            return default(T);
        }

        public Task Task()
        {
            return null;
        }

        public Task<string> TaskString()
        {
            return null;
        }

        public Task<Nullable<int>> TaskNullableInt()
        {
            return null;
        }

        public void ArrayParameter(byte[] byteArray)
        {
        }

        public void DefaultValueParameter(string nullValue = null, string stringValue = "str\\ing\"quotes\"", bool boolValue = true)
        {
        }
    }

    public class GenericMethodInfo<T>
    {
        public T Method(T parameter)
        {
            return default(T);
        }

        public T1 Generic<T1>(T1 parameter1, T parameter2)
        {
            return default(T1);
        }
    }
}
