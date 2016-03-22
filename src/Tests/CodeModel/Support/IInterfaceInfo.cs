namespace Typewriter.Tests.CodeModel.Support
{
    /// <summary>
    /// summary
    /// </summary>
    [AttributeInfo]
    public interface IInterfaceInfo : IBaseInterfaceInfo
    {
        event Delegate PublicEvent;
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

    public interface IInheritGenericInterfaceInfo : IGenericInterface<string>
    {
    }

    public class InterfaceContiningClassInfo
    {
        public interface INestedInterfaceInfo
        {
        }
    }
}