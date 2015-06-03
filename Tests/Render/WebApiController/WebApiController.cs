using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tests.Render.WebApiController.Support;

namespace Tests.Render.WebApiController
{
    public class WebApiController
    {
        [HttpGet]
        public async Task<IHttpActionResult<ICollection<ComplexClassModel>>> Read()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IHttpActionResult<ComplexClassModel>> Create(ComplexClassModel user)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        public async Task<IHttpActionResult<ComplexClassModel>> Update(ComplexClassModel user, int id)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        public async Task<IHttpActionResult<HttpStatusCode>> Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
