using Should;
using Typewriter.CodeModel.CodeDom;
using Typewriter.CodeModel.Providers;
using Typewriter.Generation;
using Typewriter.Tests.Render.RoutedApiController;
using Typewriter.Tests.TestInfrastructure;
using Xunit;

namespace Typewriter.Tests.Render
{
    [Trait("Render", "CodeDom")]
    public class CodeDomRenderTests : RenderTests<CodeDomCodeModelProvider>
    {
    }

    [Trait("Render", "Roslyn")]
    public class RoslynRenderTests : RenderTests<RoslynProviderStub>
    {
    }

    public abstract class RenderTests<T> : TestBase<T> where T : ICodeModelProvider, new()
    {
        private void Assert<TClass>()
        {
            var type = typeof(TClass);
            var nsParts = type.FullName.Remove(0, 11).Split('.');

            var path = string.Join(@"\", nsParts);

            var template = new Template(GetProjectItem(path + ".tstemplate"));
            var model = codeModelProvider.GetFile(GetProjectItem(path + ".cs"));
            var result = GetFileContents(path + ".result");

            bool success;
            var output = template.Render(model, out success);
            
            success.ShouldBeTrue();
            output.ShouldEqual(result);
        }

        [Fact]
        public void Expect_webapi_controller_to_angular_service_to_render_correctly()
        {
            Assert<WebApiController.WebApiController>();
        }

        [Fact]
        public void Expect_routed_webapi_controller_to_render_correctly()
        {
            Assert<BooksController>();
        }
    }
}
