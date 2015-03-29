using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Models
{
    public class ComplexGenericUseModel
    {
        public ComplexGenericModel<int> Generic { get; set; }
        public ICollection<ComplexGenericModel<int>> EnumerableGeneric { get; set; }
    }
}
