using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Typewriter.Tests.Render.WebApiController.Support;
using Typewriter.Tests.Support;

namespace Typewriter.Tests.Render.WebApiController
{
    public class WebApiController
    {
        [HttpGet]
        public Task<IHttpActionResult<ICollection<ComplexClassModel>>> Read()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public Task<IHttpActionResult<ComplexClassModel>> Create(ComplexClassModel user)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        public Task<IHttpActionResult<ComplexClassModel>> Update(ComplexClassModel user, int id)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        public Task<IHttpActionResult<HttpStatusCode>> Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
