#pragma warning disable 67
namespace Typewriter.Tests.CodeModel.Support
{
    public partial class PartialClassInfo
    {
        public PartialClassInfo(string x)
        {
        }

        public const string Constant2 = "";
        public delegate void Delegate2<T>(string param1, T param2);
        public event Delegate Event2;
        public string Field2 = "";
        public void Method2() { }
        public string Property2 { get; set; }

        public class NestedClassInfo2
        {
            public string NestedProperty2 { get; set; }
        }

        public interface INestedInterfaceInfo2
        {
            string NestedProperty2 { get; set; }
        }

        public enum NestedEnumInfo2
        {
            NestedValue2
        }
    }
}
