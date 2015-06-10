using System;

namespace Tests.Render.Support
{
    internal class RouteAttribute : Attribute
    {
        private string v;

        public RouteAttribute(string v)
        {
            this.v = v;
        }
    }
}