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
        public void complete_class_test()
       {
           var template = @"module App {$Classes(*Model)[
    export class $Name$IsGeneric[<T>][] {
        $Properties[
        public $name: $Type = $Default;]
		$Methods[
		public $name = $IsGeneric[<$GenericTypeArguments[$Type][, ]>][]($Parameters[$name: $Type][, ]) : $Type => { };]
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

        [TestMethod]
        public void webapi_controller_to_angular_service()
        {
            var template = @"module App {$Classes(*WebApiController)[    
    export class $Name$IsGeneric[<T>][] {
		$Methods[
		public $name = ($Parameters[$name: $Type][, ]) : Promise<$Type[$GenericTypeArguments[$GenericTypeArguments[$Type]]]> => {
            var verb = '$verb';
            var route = '$Route';
        };]
    }]
}";
            var extension = "${var verb = (Method m) => m.Attributes.FirstOrDefault(a => a.Name.Contains(\"Http\")).Name.Replace(\"Http\", \"\").ToLower();}";
            
            var expected = @"module App {    
    export class WebApiController {
		
		public read = () : Promise<ComplexClassModel[]> => {
            var verb = 'get';
            var route = '$Route';
        };
		public create = (user: ComplexClassModel) : Promise<ComplexClassModel> => {
            var verb = 'post';
            var route = '$Route';
        };
		public update = (user: ComplexClassModel, id: number) : Promise<ComplexClassModel> => {
            var verb = 'put';
            var route = '$Route';
        };
		public delete = (id: number) : Promise<HttpStatusCode> => {
            var verb = 'delete';
            var route = '$Route';
        };
    }
}";
            Verify<WebApiController>(extension + template, expected);
        }
    }
}
