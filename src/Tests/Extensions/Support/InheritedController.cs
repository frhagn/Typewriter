using System.Collections.Generic;
using Typewriter.Tests.Support;

namespace Typewriter.Tests.Extensions.Support
{
    public class InheritedController : BaseController
    {
        [HttpGet("inherited/{id}")]
        public IEnumerable<string> RoutePrefixFromBaseHttpGetWithParameter(int id)
        {
            return null; // just for testing
        }
    }
}