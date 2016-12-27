#pragma warning disable 67
namespace Typewriter.Tests.CodeModel.Support
{
    /// <summary>
    /// summary
    /// </summary>
    [AttributeInfo]
    public partial class PartialClassInfo
    {
        public PartialClassInfo()
        {
        }

        public const string Constant1 = "";
        public delegate void Delegate1<T>(string param1, T param2);
        public event Delegate Event1;
        public string Field1 = "";
        public void Method1() { }
        public string Property1 { get; set; }

        public class NestedClassInfo1
        {
            public string NestedProperty1 { get; set; }
        }

        public interface INestedInterfaceInfo1
        {
            string NestedProperty1 { get; set; }
        }

        public enum NestedEnumInfo1
        {
            NestedValue1
        }
    }
}
