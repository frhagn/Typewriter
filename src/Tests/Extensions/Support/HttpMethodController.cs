using System.Collections.Generic;
using Typewriter.Tests.Support;

namespace Typewriter.Tests.Extensions.Support
{
    public class HttpMethodController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return null;
        }

        // GET api/values
        public IEnumerable<string> GetAll()
        {
            return null;
        }

        // GET api/values
        public IEnumerable<string> ListAll()
        {
            return null;
        }

        // POST api/values/5
        [HttpPost]
        public string GetHttpAttibute(int id)
        {
            return null;
        }

        // HEAD api/values/5
        [AcceptVerbs("HEAD")]
        public string GetAcceptVerbsAttribute(int id)
        {
            return null;
        }

        // POST api/values/5
        [AcceptVerbs("HEAD", "POST")]
        public string GetMultipleAcceptVerbsAttribute1(int id)
        {
            return null;
        }

        // HEAD api/values/5
        [AcceptVerbs("HEAD", "PUT")]
        public string GetMultipleAcceptVerbsAttribute2(int id)
        {
            return null;
        }
    }
}
