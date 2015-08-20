using System;
using System.Collections.Generic;
using System.Net;
using Typewriter.Tests.Render.RoutedApiController.Support;
using Typewriter.Tests.Support;

namespace Typewriter.Tests.Render.RoutedApiController
{
    [RoutePrefix("api/library/{libraryId:int}/books")]
    public class BooksController
    {
        [Route(""), HttpGet]
        public IEnumerable<Book> Get() {throw new NotImplementedException(); }

        [Route("{id:int}"), HttpGet]
        public Book GetById(int id) { throw new NotImplementedException(); }

        [Route("{id:int}"), HttpGet]
        public Book GetByLibraryAndId(int libraryId, int id) { throw new NotImplementedException(); }

        [Route(""), HttpPost]
        public HttpStatusCode Post([FromBody]Book book, string query1, bool query2) { throw new NotImplementedException(); }
    }
}
