using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Models
{
    public class ComplexClassModel
    {
        public ComplexClass Complex { get; set; }
        public ICollection<ComplexClass> EnumerableComplex { get; set; }
    }
}
