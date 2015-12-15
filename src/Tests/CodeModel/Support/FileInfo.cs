
public class PublicClassNoNamespace
{
    public class PublicNestedClassNoNamespace
    {
    }

    public delegate void PublicNestedDelegateNoNamespace();

    public enum PublicNestedEnumNoNamespace
    {
    }

    public interface PublicNestedInterfaceNoNamespace
    {
    }
}

internal class InternalClassNoNamespace
{
}

public delegate void PublicDelegateNoNamespace();
internal delegate void InternalDelegateNoNamespace();

public enum PublicEnumNoNamespace
{
}

internal enum InternalEnumNoNamespace
{
}

public interface PublicInterfaceNoNamespace
{
}

internal interface InternalInterfaceNoNamespace
{
}

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
