using System;

namespace Typewriter.Tests.Support
{
    public class HttpGetAttribute : Attribute
    {
        public HttpGetAttribute()
        {
        }

        public HttpGetAttribute(string route)
        {
        }
    }
}