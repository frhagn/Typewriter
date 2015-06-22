namespace Typewriter.Tests.CodeModel.Support
{
    public class MethodInfo
    {
        [AttributeInfo]
        public void Method([AttributeInfo]string parameter)
        {
        }

        public T Generic<T>(T parameter)
        {
            return default(T);
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
