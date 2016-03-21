using System;

namespace Typewriter.Tests.Support
{
    internal class RouteAttribute : Attribute
    {
        private string v;

        public RouteAttribute(string v)
        {
            this.v = v;
        }

        public string Name { get; set; }
    }
}