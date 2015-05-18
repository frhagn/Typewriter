using System;
using EnvDTE;
using Should;
using Typewriter.CodeModel.CodeDom;
using Typewriter.Generation;

namespace Tests
{
    public class RenderTests : TestBase
    {
        public void Test1()
        {
            var template = new Template(GetProjectItem(@"Tests\Models\TestModel1.tstemplate"));
            var model = new FileInfo(GetProjectItem(@"Tests\Models\TestModel1.cs"));
            var result = GetFileContents(@"Tests\Models\TestModel1.result");

            bool success;
            var output = template.Render(model, out success);

            success.ShouldBeTrue();
            output.ShouldEqual(result);
        }
    }
}
