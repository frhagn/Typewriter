using System.Linq;
using Typewriter.CodeModel;
using Typewriter.CodeModel.Attributes;

namespace Typewriter.Extensions
{
    /// <summary>
    /// Extension methods for working with the Typewriter CodeModel.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns the value of the RouteAttribute of the method and the RoutePrefixAttribute of the current class
        /// </summary>
        //[Property("(extension) string Route", "Returns the value of the RouteAttribute of the $context and the RoutePrefixAttribute of the current class")]
        public static string Route(this Method methodInfo)
        {
            var parent = methodInfo.Parent as Class;
            var route = methodInfo.Attributes.FirstOrDefault(a => a.Name == "Route");
            var routePrefix = parent?.Attributes.FirstOrDefault(a => a.Name == "RoutePrefix");

            if (route == null && routePrefix == null) return null;
            
            return string.Concat(routePrefix != null ? routePrefix.Value + "/" : null, route?.Value);
        }
   
    }
}