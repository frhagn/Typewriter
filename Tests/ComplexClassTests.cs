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
                "$Classes(*Model)[$Name$IsGeneric[<T>][]]",
                "ComplexGenericModel<T>");
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
