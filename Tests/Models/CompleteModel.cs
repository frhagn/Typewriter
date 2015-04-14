using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Models
{
    public class CompleteModel
    {
        public int PrimitiveProperty { get; set; }
        public ComplexClass ComplexProperty { get; set; }
        public ComplexGenericModel<ComplexClass> ComplexGenericProperty { get; set; }
        public ICollection<ComplexGenericModel<ComplexClass>> EnumerableGenericProperty { get; set; }

        public int PrimitiveMethod(string stringArg, int intArg)
        {
            throw new NotImplementedException();
        }

        public void VoidMethod()
        {

        }

        public ICollection<ComplexGenericModel<T>> GenericMethod<T>()
        {
            throw new NotImplementedException();
        }
    }
}
