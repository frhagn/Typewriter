namespace Typewriter.Tests.CodeModel.Support
{
    [AttributeInfo]
    public interface IInterfaceInfo : IBaseInterfaceInfo
    {
        void PublicMethod();
        string PublicProperty { get; set; }
    }

    public interface IBaseInterfaceInfo
    {
        string PublicBaseProperty { get; set; }
    }

    public interface IGenericInterface<T>
    {
    }

    public class InterfaceContiningClassInfo
    {
        public interface INestedInterfaceInfo
        {
        }
    }
}