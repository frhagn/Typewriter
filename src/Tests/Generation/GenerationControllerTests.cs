using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using NSubstitute;
using Should;
using Typewriter.Generation;
using Typewriter.Generation.Controllers;
using Typewriter.Tests.Render.WebApiController;
using Typewriter.Tests.TestInfrastructure;
using Xunit;
using File = Typewriter.CodeModel.File;

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

            var templateController = new TemplateController(dte, item => new TemplateSubstitute(item));

            var generationController = new GenerationController(dte, metadataProvider, templateController, eventQueue);

            var type = typeof(WebApiController);
            var nsParts = type.FullName.Remove(0, 11).Split('.');

            var path = string.Join(@"\", nsParts);

            var templateFilename = Path.Combine(SolutionDirectory, path + ".tstemplate");

            generationController.OnTemplateChanged(templateFilename);

            
           
        }
    }

    public class TemplateSubstitute : Template
    {
        public TemplateSubstitute(ProjectItem projectItem) : base(projectItem)
        {
        }

        public override void SaveProjectFile()
        {
            Trace.WriteLine("SaveProjectFile intercepted");
        }

        protected override void SaveFile(File file, string output)
        {
            Trace.WriteLine("SaveFile intercepted");
            Trace.WriteLine("Output: ");
            Trace.WriteLine(output);
        }
    }
}