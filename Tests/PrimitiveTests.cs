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
    [TestClass]
    public class PrimitiveTests : TestBase
    {
        [TestMethod]
        public void test_simple_nullable_enumerable_nullable()
        {
            Verify<PrimitiveNullableEnumerableModel>(
                "$Classes(*Model)[$Properties[$Type ]",
                "boolean[] number[] Date[] ");
        }
        [TestMethod]
        public void test_simple_nullable()
        {
            Verify<PrimitiveNullableModel>(
                "$Classes(*Model)[$Properties[$Type ]",
                "boolean number Date ");
        }
        [TestMethod]
        public void test_simple_enumerable_types()
        {
            Verify<PrimitiveEnumerableModel>(
                "$Classes(*Model)[$Properties[$Type ]",
                "boolean[] number[] string[] Date[] ");
        }

        [TestMethod]
        public void test_simple_property_types()
        {
            Verify<PrimitiveModel>("$Classes(*Model)[$Properties[$Type ]",
                "boolean number string Date ");
        }

        [TestMethod]
        public void test_simple_property_names()
        {
            Verify<PrimitiveModel>("$Classes(*Model)[$Properties[$Name ]", 
                "BoolProperty NumberProperty StringProperty DateTimeProperty ");
        }
        [TestMethod]
        public void test_simple_class_Name()
        {
            Verify<PrimitiveModel>("$Classes(*Model)[$Name]", "PrimitiveModel");
        }

    }
   
}
