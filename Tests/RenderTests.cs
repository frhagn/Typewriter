using System;
using EnvDTE;
using Should;
using Typewriter.CodeModel.CodeDom;
using Typewriter.Generation;
using Tests.Models;
using Tests.Render.WebApiController;
using Tests.Render.RoutedApiController;
using System.Diagnostics;

namespace Tests
{
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

        public void Test1()
        {
            Assert<TestModel1>();
        }

        public void webapi_controller_to_angular_service()
        {
            Assert<WebApiController>();
        }

        public void routed_webapi_controller()
        {
            Assert<BooksController>();
        }
    }
}
