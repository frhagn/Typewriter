using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Typewriter.Generation;
using System.Collections.Generic;
using Typewriter.CodeModel;
using Typewriter.CodeModel.CodeDom;
using Typewriter.Generation.Controllers;
using System.Diagnostics;
using System.Linq;
using EnvDTE;
using Typewriter.VisualStudio;
using Tests.Models;

namespace Tests
{
    public abstract class TestBase
    {
        public void VerifyExplicit(string name, string template, string expectedOutput)
        {
            EnvDTE80.DTE2 dte2;
            dte2 = (EnvDTE80.DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.12.0");
            var projectItems = dte2.Solution
                .AllProjetcs()
                .Single(p => p.Name == "Tests")
                .AllProjectItems();

            var projectItem = projectItems.Single(p => p.Path().EndsWith("Models\\" + name + ".cs"));
            bool success = false;
            var output = Parser.Parse(template, null, new FileInfo(projectItem), out success);

            Assert.AreEqual(expectedOutput, output);
        }
        public void Verify<T>(string template, string expectedOutput)
        {
            var t = typeof(T);
            VerifyExplicit(t.Name, template, expectedOutput);
        }
    }
}
