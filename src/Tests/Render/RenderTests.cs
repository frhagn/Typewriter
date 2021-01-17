using Microsoft.VisualStudio.Sdk.TestFramework;
using Should;
using Typewriter.Generation;
using Typewriter.Tests.TestInfrastructure;
using Xunit;

namespace Typewriter.Tests.Render
{
    [Trait("Render", "CodeDom"), Collection(nameof(CodeDomFixture))]
    public class CodeDomRenderTests : RenderTests
    {
        public CodeDomRenderTests(CodeDomFixture fixture, GlobalServiceProvider sp) : base(fixture, sp)
        {
        }
    }

    [Trait("Render", "Roslyn"), Collection(nameof(RoslynFixture))]
    public class RoslynRenderTests : RenderTests
    {
        public RoslynRenderTests(RoslynFixture fixture, GlobalServiceProvider sp) : base(fixture, sp)
        {
        }
    }

    public abstract class RenderTests : TestBase
    {
        protected RenderTests(ITestFixture fixture, GlobalServiceProvider sp) : base(fixture, sp)
        {
        }

        private void Assert<TClass>()
        {
            var type = typeof(TClass);
            var nsParts = type.FullName.Remove(0, 11).Split('.');

            var path = string.Join(@"\", nsParts);

            var template = new Template(GetProjectItem(path + ".tstemplate"));
            var file = GetFile(path + ".cs");
            var result = GetFileContents(path + ".result");

            var output = template.Render(file, out var success);
            
            success.ShouldBeTrue();
            output.ShouldEqual(result);
        }

        //[Fact]
        //public void Expect_webapi_controller_to_angular_service_to_render_correctly()
        //{
        //    Assert<WebApiController.WebApiController>();
        //}

        //[Fact]
        //public void Expect_routed_webapi_controller_to_render_correctly()
        //{
        //    Assert<BooksController>();
        //}
    }
}
