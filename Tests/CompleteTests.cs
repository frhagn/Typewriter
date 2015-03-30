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
    public class CompleteTests : TestBase
    {
        [TestMethod]
        public void test()
        {
            var template = @"module App {$Classes(*Model)[
    export class $Name$IsGeneric[<T>][] {
        $Properties[
        public $name: $Type = $Default;]
		$Methods[
		public $name = $IsGeneric[<T>][]($Parameters[$name: $Type][, ]) : $Type => { };]
    }]
}";
            var expected = @"module App {
    export class CompleteModel {
        
        public primitiveProperty: number = 0;
        public complexProperty: ComplexClass = null;
        public complexGenericProperty: ComplexGenericModel<ComplexClass> = null;
        public enumerableGenericProperty: ComplexGenericModel<ComplexClass>[] = [];
		
		public primitiveMethod = (stringArg: string, intArg: number) : number => { };
		public voidMethod = () : void => { };
		public genericMethod = <T>() : ComplexGenericModel<T>[] => { };
    }
}";
            Verify<CompleteModel>(template, expected);
        }
    }
}
