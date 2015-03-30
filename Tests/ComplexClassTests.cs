using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Models;

namespace Tests
{
    [TestClass]
    public class ComplexClassTests : TestBase
    {
        [TestMethod]
        public void test_complex_classes()
        {
            Verify<ComplexClassModel>(
                "$Classes(*Model)[$Properties[$Name $Type ]",
                "Complex ComplexClass EnumerableComplex ComplexClass[] ");
        }

        [TestMethod]
        public void test_task_complex_classes()
        {
            Verify<ComplexTaskModel>(
                "$Classes(*Model)[$Properties[$Name $Type ]",
                "Task ComplexClass ");
        }

        [TestMethod]
        public void test_complex_generic_class()
        {
            VerifyExplicit("ComplexGenericModel",
                "$Classes(*Model)[$Name$IsGeneric[<$GenericTypeArguments[$Type][, ]>][]]",
                "ComplexGenericModel<T>");
        }

        [TestMethod]
        public void test_stuff()
        {
            //Tests.Models.ComplexGenericModel<T>
            var projectItem = GetFileInfo("ComplexGenericModel");
            var args = projectItem.Classes.Single().GenericTypeArguments;
        }

        [TestMethod]
        public void test_complex_multi_generic_class()
        {
            VerifyExplicit("ComplexMultiGenericModel",
                "$Classes(*Model)[$Name$IsGeneric[<$GenericTypeArguments[$Type][,]>][]]",
                "ComplexMultiGenericModel<T1, T2, T3>");
        }

        [TestMethod]
        public void test_complex_generic_property()
        {
            VerifyExplicit("ComplexGenericModel",
                "$Classes(*Model)[$Properties[$Type]",
                "T");
        }
        [TestMethod]
        public void test_complex_generic_properties()
        {
            Verify<ComplexGenericUseModel>(
                "$Classes(*Model)[$Properties[$Name $Type ]",
                "Generic ComplexGenericModel<number> EnumerableGeneric ComplexGenericModel<number>[] ");
        }
    }
}
