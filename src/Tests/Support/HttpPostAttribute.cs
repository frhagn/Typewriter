using System;

namespace Typewriter.Tests.Support
{
    public class HttpPostAttribute : Attribute
    {
        public HttpPostAttribute()
        {
        }

        public HttpPostAttribute(string route)
        {
        }
    }
}