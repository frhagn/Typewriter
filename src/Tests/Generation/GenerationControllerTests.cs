using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Should;
using Typewriter.Generation.Controllers;
using Typewriter.Tests.Render.WebApiController;
using Typewriter.Tests.TestInfrastructure;
using Xunit;

namespace Typewriter.Tests.Generation
{
    [Trait("OnTemplateChanged", "Roslyn"), Collection(nameof(RoslynFixture))]
    public class GenerationControllerTests : TestBase
    {

        public GenerationControllerTests(RoslynFixture fixture) : base(fixture)
        {

        }

        [Fact]
        public void Expect_GenerateTemplate()
        {
            var eventQueue = Substitute.For<IEventQueue>();
            eventQueue.When(m => m.Enqueue(Arg.Any<Action>())).Do(c =>
            {
                Trace.WriteLine("Enqueue Called");
                c.Arg<Action>()();
            });

            var controller = new GenerationController(dte, metadataProvider, new TemplateController(dte), eventQueue);

            var type = typeof(WebApiController);
            var nsParts = type.FullName.Remove(0, 11).Split('.');

            var path = string.Join(@"\", nsParts);

            var filename = Path.Combine(SolutionDirectory, path + ".tstemplate");

            controller.OnTemplateChanged(filename);


        }
    }
}