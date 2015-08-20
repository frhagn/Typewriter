using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Typewriter.Tests.Support;

namespace Typewriter.Tests.Extensions.Support
{
    public class RouteController
    {
        [Route("api/{controller}/a/{name}/{id?}")]
        public string GetRoute(string name, string filter = null, int count = 0)
        {
            return null;
        }
    }
}
