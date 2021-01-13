using Should;
using Xunit;

namespace Typewriter.Tests.Helpers
{
    [Trait("Helpers", "CamelCase")]
    public class CamelCaseTests
    {
        [Fact]
        public void Expect_strings_to_be_camel_cased_correctly()
        {
            // Tests from Json.NET
            // https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json.Tests/Utilities/StringUtilsTests.cs
            Typewriter.CodeModel.Helpers.CamelCase("URLValue").ShouldEqual("urlValue");
            Typewriter.CodeModel.Helpers.CamelCase("URL").ShouldEqual("url");
            Typewriter.CodeModel.Helpers.CamelCase("ID").ShouldEqual("id");
            Typewriter.CodeModel.Helpers.CamelCase("I").ShouldEqual("i");
            Typewriter.CodeModel.Helpers.CamelCase("").ShouldEqual("");
            Typewriter.CodeModel.Helpers.CamelCase(null).ShouldEqual(null);
            Typewriter.CodeModel.Helpers.CamelCase("Person").ShouldEqual("person");
            Typewriter.CodeModel.Helpers.CamelCase("iPhone").ShouldEqual("iPhone");
            Typewriter.CodeModel.Helpers.CamelCase("IPhone").ShouldEqual("iPhone");
            Typewriter.CodeModel.Helpers.CamelCase("I Phone").ShouldEqual("i Phone");
            Typewriter.CodeModel.Helpers.CamelCase("I  Phone").ShouldEqual("i  Phone");
            Typewriter.CodeModel.Helpers.CamelCase(" IPhone").ShouldEqual(" IPhone");
            Typewriter.CodeModel.Helpers.CamelCase(" IPhone ").ShouldEqual(" IPhone ");
            Typewriter.CodeModel.Helpers.CamelCase("IsCIA").ShouldEqual("isCIA");
            Typewriter.CodeModel.Helpers.CamelCase("VmQ").ShouldEqual("vmQ");
            Typewriter.CodeModel.Helpers.CamelCase("Xml2Json").ShouldEqual("xml2Json");
            Typewriter.CodeModel.Helpers.CamelCase("SnAkEcAsE").ShouldEqual("snAkEcAsE");
            Typewriter.CodeModel.Helpers.CamelCase("SnA__kEcAsE").ShouldEqual("snA__kEcAsE");
            Typewriter.CodeModel.Helpers.CamelCase("SnA__ kEcAsE").ShouldEqual("snA__ kEcAsE");
            Typewriter.CodeModel.Helpers.CamelCase("already_snake_case_ ").ShouldEqual("already_snake_case_ ");
            Typewriter.CodeModel.Helpers.CamelCase("IsJSONProperty").ShouldEqual("isJSONProperty");
            Typewriter.CodeModel.Helpers.CamelCase("SHOUTING_CASE").ShouldEqual("shoutinG_CASE");
            Typewriter.CodeModel.Helpers.CamelCase("9999-12-31T23:59:59.9999999Z").ShouldEqual("9999-12-31T23:59:59.9999999Z");
            Typewriter.CodeModel.Helpers.CamelCase("Hi!! This is text. Time to test.").ShouldEqual("hi!! This is text. Time to test.");
        }
    }
}
