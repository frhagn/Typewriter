using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Typewriter.Tests.Support;

namespace Typewriter.Tests.Extensions.Support
{
    [Route("api/[controller]")]
    public class RouteControllerWithDefaultRouteController
    {
        [Route("a/{name}/{id?}")]
        public string GetRoute(string name, string filter = null, int count = 0)
        {
            return null;
        }

        [Route("{*key:string}")]
        public string WildcardRoute(string key)
        {
            return null;
        }

        [Route("{id}", Name = "name")]
        public string NamedRoute(int id)
        {
            return null;
        }

        [HttpGet("{id}")]
        public string RouteInHttpAttribute(int id)
        {
            return null;
        }

        [HttpGet("sub/{id}")]
        public string SubRouteInHttpAttribute(int id)
        {
            return null;
        }

    }
}
