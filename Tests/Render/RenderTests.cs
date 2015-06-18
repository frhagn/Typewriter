﻿using Should;
using Tests.Render.RoutedApiController;
using Tests.TestInfrastructure;
using Typewriter.CodeModel.CodeDom;
using Typewriter.Generation;
using Xunit;

namespace Tests.Render
{
    [Trait("Render", null)]
    public class RenderTests : TestBase
    {
        private void Assert<T>()
        {
            var type = typeof(T);
            var nsParts = type.FullName.Split('.');

            var path = string.Join(@"\", nsParts);

            var template = new Template(GetProjectItem(path + ".tstemplate"));
            var model = new FileInfo(GetProjectItem(path + ".cs"));
            var result = GetFileContents(path + ".result");

            bool success;
            var output = template.Render(model, out success);
            
            success.ShouldBeTrue();
            output.ShouldEqual(result);
        }

        [Fact]
        public void webapi_controller_to_angular_service()
        {
            Assert<WebApiController.WebApiController>();
        }

        [Fact]
        public void routed_webapi_controller()
        {
            Assert<BooksController>();
        }
    }
}