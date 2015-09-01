namespace Typewriter.Tests.CodeModel.Support
{
    public class PublicClass
    {
        public class PublicNestedClass
        {
        }

        public delegate void PublicNestedDelegate();

        public enum PublicNestedEnum
        {
        }

        public interface PublicNestedInterface
        {
        }
    }

    internal class InternalClass
    {
    }

    public delegate void PublicDelegate();
    internal delegate void InternalDelegate();

    public enum PublicEnum
    {
    }

    internal enum InternalEnum
    {
    }

    public interface PublicInterface
    {
    }

    internal interface InternalInterface
    {
    }
}
