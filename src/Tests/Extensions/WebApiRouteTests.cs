using System.Linq;
using Should;
using Typewriter.CodeModel;
using Typewriter.Extensions.WebApi;
using Typewriter.Tests.TestInfrastructure;
using Xunit;

namespace Typewriter.Tests.Extensions
{
    [Trait("Extensions", "WebApi"), Collection(nameof(CodeDomFixture))]
    public class CodeDomWebApiRouteExtensionsTests : WebApiRouteExtensionsTests
    {
        public CodeDomWebApiRouteExtensionsTests(CodeDomFixture fixture) : base(fixture)
        {
        }
    }

    [Trait("Extensions", "WebApi"), Collection(nameof(RoslynFixture))]
    public class RoslynWebApiRouteExtensionsTests : WebApiRouteExtensionsTests
    {
        public RoslynWebApiRouteExtensionsTests(RoslynFixture fixture) : base(fixture)
        {
        }
    }

    public abstract class WebApiRouteExtensionsTests : TestBase
    {
        private readonly File fileInfo;

        protected WebApiRouteExtensionsTests(ITestFixture fixture) : base(fixture)
        {
            fileInfo = GetFile(@"Tests\Extensions\Support\RouteController.cs");
        }

        [Fact]
        public void Expect_to_find_parameters_on_wildcard_route_url()
        {
            var classInfo = fileInfo.Classes.First();
            var methodInfo = classInfo.Methods.First(p => p.Name == "WildcardRoute");

            methodInfo.Url().ShouldEqual("api/${key}");
        }

        [Fact]
        public void Expect_to_find_url_on_named_route()
        {
            var classInfo = fileInfo.Classes.First();
            var methodInfo = classInfo.Methods.First(p => p.Name == "NamedRoute");

            methodInfo.Url().ShouldEqual("api/${id}");
        }

        [Fact]
        public void Expect_to_find_url_on_httpget_route()
        {
            var classInfo = fileInfo.Classes.First();
            var methodInfo = classInfo.Methods.First(p => p.Name == "HttpGetRoute");

            methodInfo.Url().ShouldEqual("api/${id}");
        }

        
    }
}