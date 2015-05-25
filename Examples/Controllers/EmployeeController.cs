using System;
using System.Collections.Generic;
using System.Linq;

namespace Typewriter.Examples.Controllers
{
    public class EmployeeController
    {
        [HttpPost]
        public string Find(int id)
        {
            
        }
    }

    public class HttpPostAttribute : Attribute
    {
    }
}