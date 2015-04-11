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
        protected ProjectItem GetProjectItem(string name)
        {
            EnvDTE80.DTE2 dte2;
            dte2 = (EnvDTE80.DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.12.0");
            var projectItems = dte2.Solution
                .AllProjetcs()
                .Single(p => p.Name == "Tests")
                .AllProjectItems();

            return projectItems.Single(p => p.Path().EndsWith("Models\\" + name + ".cs"));
        }

        protected FileInfo GetFileInfo(string name)
        {
            var projectItem = GetProjectItem(name);
            return new FileInfo(projectItem);
        }

        public void VerifyExplicit(string name, string template, string expectedOutput)
        {
            var fileInfo = GetFileInfo(name);
            bool success = false;
            
            System.Type extensions = null;
            var t = TemplateParser.Parse(template, ref extensions);

            var output = Parser.Parse(t, extensions, fileInfo, out success);
            
            Assert.AreEqual(expectedOutput, output);
        }
        public void Verify<T>(string template, string expectedOutput)
        {
            var t = typeof(T);
            VerifyExplicit(t.Name, template, expectedOutput);
        }
    }
}
