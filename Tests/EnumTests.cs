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
    public class EnumTests : TestBase
    {
        [TestMethod]
        public void test_enum_values()
        {
            Verify<EnumModel>(
                "$Enums(*Model)[$Values[$Name $Value ]",
                "TestValue1 1 TestValue2 2 TestValue3 3 ");
        }

        [TestMethod]
        public void test_enum_property()
        {
            Verify<EnumClassModel>(
                "$Classes(*Model)[$Properties[$Name $Type ]",
                "EnumProperty EnumModel ");
        }
    }
}
